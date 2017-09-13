using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using HaereRa.API.DAL;
using HaereRa.Plugin;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Loader;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
    public class SuggestionsService : ISuggestionService
    {
        private readonly HaereRaDbContext _dbContext;
        private readonly IPersonService _personService;

        public SuggestionsService(HaereRaDbContext dbContext, IPersonService personService)
        {
            _dbContext = dbContext;
            _personService = personService;
        }

        public async Task AcceptSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default(CancellationToken))
        {
			cancellationToken.ThrowIfCancellationRequested();
			if (suggestionId <= 0) throw new ArgumentOutOfRangeException(nameof(suggestionId), "suggestionId must be larger than zero.");

			var suggestion = await _dbContext.ProfileSuggestions.Where(s => s.Id == suggestionId).FirstOrDefaultAsync();
			if (suggestion == null) throw new KeyNotFoundException("Suggestion does not exist in the database.");

            var alreadyExistsAsProfile = await _dbContext.Profiles
                .Where(p => 
                    p.PersonId == suggestion.PersonId &&
                    p.ProfileTypeId == suggestion.ProfileTypeId &&
                    p.ProfileAccountIdentifier == suggestion.ProfileAccountIdentifier)
                .AnyAsync(cancellationToken);
            
			if (!alreadyExistsAsProfile)
            {
                await _dbContext.Profiles.AddAsync(
                    new Profile{
	                    PersonId = suggestion.PersonId,
	                    ProfileTypeId = suggestion.ProfileTypeId,
	                    ProfileAccountIdentifier = suggestion.ProfileAccountIdentifier,
                    },
                    cancellationToken
                );
            }

            suggestion.IsAccepted = ProfileSuggestionStatus.Accepted;
			await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSuggestionsAsync(int personId, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO: Move to ProfileService
            var allInspectableProfileTypes = await _dbContext.ProfileTypes
                                                             .Where(p => !String.IsNullOrWhiteSpace(p.PluginAssembly))
                                                             .ToListAsync();

            // TODO: Move to PersonService
            var alreadyAcceptedProfileTypeIds = await _dbContext.Profiles
                                                        .Where(p => p.PersonId == personId)
                                                        .Select(p => p.ProfileTypeId)
                                                        .Distinct()
                                                                .ToListAsync(cancellationToken);

            var person = await _personService.GetPersonAsync(personId, cancellationToken);

            var profileTypesToCheck = new List<ProfileType>();

            var possibleUsernamesForUser = await GetPossibleUsernamesAsync(
                person.FullName,
                person.KnownAs,
                dashesAllowed: true,
                dotsUsed: true,
                cancellationToken: cancellationToken);
            
            foreach (var profileType in allInspectableProfileTypes)
            {
                if (!alreadyAcceptedProfileTypeIds.Contains(profileType.Id))
                {
                    profileTypesToCheck.Add(profileType);
                }
            }

            foreach (var profileType in profileTypesToCheck)
            {
                var pluginAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath($"{profileType.PluginAssembly}.dll");
                var pluginEntryType = pluginAssembly.GetType($"HaereRa.Plugin.{profileType.PluginAssembly}.{profileType.PluginAssembly}ExternalAccountPlugin");

                IFetchExternalUserAccounts pluginInstance;
                if (!String.IsNullOrWhiteSpace(profileType.PluginAssemblyOptions))
                {
                    // Get instance of the plugin with the defined options (based as the first ctor parameter)
                    var constructorArguments = new object[] { profileType.PluginAssemblyOptions };

                    pluginInstance = Activator.CreateInstance(pluginEntryType, constructorArguments) as IFetchExternalUserAccounts;
                }
                else
                {
					// Get instance of the plugin with default options
					pluginInstance = Activator.CreateInstance(pluginEntryType) as IFetchExternalUserAccounts;
                }

                var allProfileTypeUsernamesList = await pluginInstance.ListProfilesAsync();
                var possibleUsernameMatches = allProfileTypeUsernamesList.Select(p => p.AccountIdentifier).Intersect(possibleUsernamesForUser, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

                var existingProfileSuggestions = await _dbContext.ProfileSuggestions
                                                       .Where(p => p.PersonId == personId && p.ProfileTypeId == profileType.Id)
                                                       .Select(p => p.ProfileAccountIdentifier)
                                                       .Distinct((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
                                                       .ToListAsync(cancellationToken);

                var newProfileSuggestions = possibleUsernameMatches.Except(existingProfileSuggestions, (IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);

                foreach (var newProfileSuggestion in newProfileSuggestions)
                {
                    await AddPossibleUsernameAsync(personId, profileType.Id, newProfileSuggestion, cancellationToken);
                }
            }
        }

        public async Task AddPossibleUsernameAsync(int personId, int profileTypeId, string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            var profileSuggestion = new ProfileSuggestion
            {
                PersonId = personId,
                ProfileTypeId = profileTypeId,
                ProfileAccountIdentifier = username,
                IsAccepted = ProfileSuggestionStatus.Pending,
            };

            _dbContext.ProfileSuggestions.Add(profileSuggestion);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false, CancellationToken cancellationToken = default(CancellationToken))
        {
			// TODO: Also generate email addresses, based off known email address (include strip dots, all characters between '+' and '@', etc)

			if (String.IsNullOrWhiteSpace(fullName)) throw new ArgumentNullException(nameof(fullName));
            cancellationToken.ThrowIfCancellationRequested();

            // Split all parts of the full name into parts
            var allNames = fullName.ToLower().Split(' ').ToList();

			// Add in nicknames/known as names if they don't already exist, if available
			if (!String.IsNullOrWhiteSpace(knownAs))
            {
                var knownAsNames = knownAs.ToLower().Split(' ');
                foreach (var knownAsName in knownAsNames)
                {
                    if (!allNames.Contains(knownAsName))
                    {
                        allNames.Add(knownAsName);
                    }
                }
            }

            var options = new ConcatenateOptions
            {
                DashesAllowed = dashesAllowed,
                DotsUsed = dotsUsed,
            };

            // Walk the tree recursively, append the word alongside one with just an initial, then keep walking
            var result = await ConcatenateWordsAsync(allNames, options, cancellationToken: cancellationToken);
            return result;
        }

        public async Task<ProfileSuggestion> GetSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default(CancellationToken))
        {
			cancellationToken.ThrowIfCancellationRequested();
			if (suggestionId <= 0) throw new ArgumentOutOfRangeException(nameof(suggestionId), "suggestionId must be larger than zero.");

			var suggestion = await _dbContext.ProfileSuggestions.Where(s => s.Id == suggestionId).FirstOrDefaultAsync();
			if (suggestion == null) throw new KeyNotFoundException("Suggestion does not exist in the database.");

			return suggestion;
        }

        public async Task RejectSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (suggestionId <= 0) throw new ArgumentOutOfRangeException(nameof(suggestionId), "suggestionId must be larger than zero.");

            var suggestion = await _dbContext.ProfileSuggestions.Where(s => s.Id == suggestionId).FirstOrDefaultAsync();
            if (suggestion == null) throw new KeyNotFoundException("Suggestion does not exist in the database.");

            suggestion.IsAccepted = ProfileSuggestionStatus.Rejected;
            await _dbContext.SaveChangesAsync();
        }

        private async Task<List<string>> ConcatenateWordsAsync(List<string> remainingWords, ConcatenateOptions options, string concatTo = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            List<string> currentResults = new List<string>();
            foreach (var word in remainingWords)
            {
                var newRemainingWords = remainingWords.ToList();
                newRemainingWords.Remove(word);

                // Add word, plus all permutations with the word next
				currentResults.Add(concatTo + word);
                currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + word, cancellationToken));

                if (options.DotsUsed && !String.IsNullOrWhiteSpace(concatTo))
                {
                    // Do the same, but with a dot seperator
					currentResults.Add(concatTo + "." + word);
					currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + word, cancellationToken));
                }

                // If the word does not contain a dash, or dashes are allowed in usernames
                if (!word.Contains("-") || options.DashesAllowed)
                {
                    // Add initial, plus all permutations with the initial next
                    currentResults.Add(concatTo + word.Substring(0, 1));
                    currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + word.Substring(0, 1), cancellationToken));

                    if (options.DotsUsed && !String.IsNullOrWhiteSpace(concatTo))
                    {
						// Do the same, but with a dot seperator
						currentResults.Add(concatTo + "." + word.Substring(0, 1));
                        currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + word.Substring(0, 1), cancellationToken));
                    }
                }

                // Apply additional logic for hyphenated words
                if (word.Contains("-"))
                {
                    // Get initials in context of the dash (e.g. "sj" for "Smith-Johnson")
                    var splitDashWord = word.Split('-').ToList();
                    string dashInitials = "";
                    foreach (var dashWord in splitDashWord)
                    {
                        dashInitials = dashInitials + dashWord.Substring(0, 1);
                    }

					// Add word without the dash, plus all permutations with the word next
					currentResults.Add(concatTo + word.Trim('-'));
                    currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + word.Trim('-'), cancellationToken));

					// Add hyphenated initials, plus all permutations with the hyphenated initials next
					currentResults.Add(concatTo + dashInitials);
                    currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + dashInitials, cancellationToken));

                    if (options.DotsUsed && !String.IsNullOrWhiteSpace(concatTo))
                    {
						// Do the same, but with a dot seperator...
						// Add word without the dash, plus all permutations with the word next
						currentResults.Add(concatTo + "." + word.Trim('-'));
						currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + word.Trim('-'), cancellationToken));

						// Add hyphenated initials, plus all permutations with the hyphenated initials next
						currentResults.Add(concatTo + "." + dashInitials);
						currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + dashInitials, cancellationToken));
                    }
                }
            }
            return currentResults.Distinct().ToList();
        }

        private class ConcatenateOptions
        {
            public bool DashesAllowed { get; set; } = true;
            public bool DotsUsed { get; set; } = false;
		}
    }
}
