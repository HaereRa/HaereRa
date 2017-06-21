using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using HaereRa.API.Models;
using HaereRa.API.DAL;
using Microsoft.EntityFrameworkCore;
using GraphQL.Execution;

namespace HaereRa.API.Services
{
    public class SuggestionsService : ISuggestionService
    {
        private readonly HaereRaDbContext _dbContext;

        public SuggestionsService(HaereRaDbContext dbContext)
        {
            _dbContext = dbContext;
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
                    new HaereRa.API.Models.Profile{
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

        public async Task<IEnumerable<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false, CancellationToken cancellationToken = default(CancellationToken))
        {
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
