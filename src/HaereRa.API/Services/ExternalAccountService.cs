using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.DAL;
using HaereRa.API.Models;
using HaereRa.Plugin;
using Microsoft.EntityFrameworkCore;

namespace HaereRa.API.Services
{
    public class ExternalAccountService : IExternalAccountService
    {
        private readonly HaereRaDbContext _dbContext;
        private readonly IPersonService _personService;

        public ExternalAccountService(HaereRaDbContext dbContext, IPersonService personService)
        {
            _dbContext = dbContext;
            _personService = personService;
        }

        public Task<IEnumerable<ExternalAccount>> GetAllAccountsForPersonAsync(int personId, bool includePendingSuggestions = false, bool includeRejectedSuggestions = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ExternalAccount>> GetAllAccountsForPlatformAsync(int externalPlatformId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ExternalAccount> GetExternalAccountAsync(int externalAccountId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task AcceptSuggestionAsync(int externalAccountId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (externalAccountId <= 0) throw new ArgumentOutOfRangeException(nameof(externalAccountId), "externalAccountId must be larger than zero.");

            var suggestion = await _dbContext.ExternalAccounts.Where(s => s.Id == externalAccountId).FirstOrDefaultAsync();
            if (suggestion == null) throw new KeyNotFoundException("External account record does not exist in the database.");
            if (suggestion.IsSuggestionAccepted == ExternalAccountSuggestionStatus.Rejected) throw new InvalidOperationException("Suggestion was already rejected.");
            if (suggestion.IsSuggestionAccepted == ExternalAccountSuggestionStatus.Accepted) throw new InvalidOperationException("Suggestion was already accepted.");

            suggestion.IsSuggestionAccepted = ExternalAccountSuggestionStatus.Accepted;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSuggestionsAsync(int personId, CancellationToken cancellationToken = default)
        {
            // Should be a separate method
            var allInspectableExternalPlatforms = await _dbContext.ExternalPlatforms
                                                             .Where(p => !String.IsNullOrWhiteSpace(p.PluginAssembly))
                                                             .ToListAsync();

            // Should be a separate method
            var alreadyAcceptedProfileTypeIds = await _dbContext.ExternalAccounts
                                                        .Where(p => p.PersonId == personId)
                                                        .Select(p => p.ExternalPlatformId)
                                                        .Distinct()
                                                        .ToListAsync(cancellationToken);

            var person = await _personService.GetPersonAsync(personId, cancellationToken);

            var profileTypesToCheck = new List<ExternalPlatform>();

            var possibleUsernamesForUser = await GetPossibleUsernamesAsync(
                person.FullName,
                person.KnownAs,
                dashesAllowed: true,
                dotsUsed: true,
                cancellationToken: cancellationToken);

            foreach (var profileType in allInspectableExternalPlatforms)
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
                var possibleUsernameMatches = allProfileTypeUsernamesList.Select(p => p.AccountIdentifier).Intersect(possibleUsernamesForUser, StringComparer.OrdinalIgnoreCase);

                var existingProfileSuggestions = await _dbContext.ExternalAccounts
                                                       .Where(p => p.PersonId == personId && p.ExternalPlatformId == profileType.Id && p.IsSuggestionAccepted == ExternalAccountSuggestionStatus.Pending)
                                                       .Select(p => p.ExternalAccountIdentifier)
                                                       .Distinct(StringComparer.OrdinalIgnoreCase)
                                                       .ToListAsync(cancellationToken);

                var newProfileSuggestions = possibleUsernameMatches.Except(existingProfileSuggestions, StringComparer.OrdinalIgnoreCase);

                foreach (var newProfileSuggestion in newProfileSuggestions)
                {
                    await AddPossibleUsernameAsync(personId, profileType.Id, newProfileSuggestion, cancellationToken);
                }
            }
        }

        public async Task AddPossibleUsernameAsync(int personId, int externalAccountId, string username, CancellationToken cancellationToken = default)
        {
            var externalAccountSuggestion = new ExternalAccount
            {
                PersonId = personId,
                ExternalPlatformId = externalAccountId,
                ExternalAccountIdentifier = username,
                IsSuggestionAccepted = ExternalAccountSuggestionStatus.Pending,
            };

            _dbContext.ExternalAccounts.Add(externalAccountSuggestion);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false, CancellationToken cancellationToken = default)
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

        public async Task RejectSuggestionAsync(int externalAccountId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (externalAccountId <= 0) throw new ArgumentOutOfRangeException(nameof(externalAccountId), "suggestionId must be larger than zero.");

            var suggestion = await _dbContext.ExternalAccounts.Where(s => s.Id == externalAccountId).FirstOrDefaultAsync();
            if (suggestion == null) throw new KeyNotFoundException("Suggestion does not exist in the database.");
            if (suggestion.IsSuggestionAccepted == ExternalAccountSuggestionStatus.Rejected) throw new InvalidOperationException("Suggestion was already rejected.");
            if (suggestion.IsSuggestionAccepted == ExternalAccountSuggestionStatus.Accepted) throw new InvalidOperationException("Suggestion was already accepted.");

            suggestion.IsSuggestionAccepted = ExternalAccountSuggestionStatus.Rejected;
            await _dbContext.SaveChangesAsync();
        }

        private async Task<List<string>> ConcatenateWordsAsync(List<string> remainingWords, ConcatenateOptions options, string concatTo = "", CancellationToken cancellationToken = default)
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
