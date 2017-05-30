using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HaereRa.API.Services
{
    public class SuggestionsService : ISuggestionService
    {
        public SuggestionsService()
        {
        }

        public async Task<List<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false)
        {
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
            var result = await ConcatenateWordsAsync(allNames, options);
            return result;
        }

        private async Task<List<string>> ConcatenateWordsAsync(List<string> remainingWords, ConcatenateOptions options, string concatTo = "")
        {
            List<string> currentResults = new List<string>();
            foreach (var word in remainingWords)
            {
                var newRemainingWords = remainingWords.ToList();
                newRemainingWords.Remove(word);

                // Add word, plus all permutations with the word next
				currentResults.Add(concatTo + word);
                currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + word));

                if (options.DotsUsed && !String.IsNullOrWhiteSpace(concatTo))
                {
					currentResults.Add(concatTo + "." + word);
					currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + word));
                }

                // If the word does not contain a dash, or dashes are allowed in usernames
                if (!word.Contains("-") || options.DashesAllowed)
                {
                    // Add initial, plus all permutations with the initial next
                    currentResults.Add(concatTo + word.Substring(0, 1));
                    currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + word.Substring(0, 1)));

                    if (options.DotsUsed && !String.IsNullOrWhiteSpace(concatTo))
                    {
                        currentResults.Add(concatTo + "." + word.Substring(0, 1));
                        currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + word.Substring(0, 1)));
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
                    currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + word.Trim('-')));

					// Add hyphenated initials, plus all permutations with the hyphenated initials next
					currentResults.Add(concatTo + dashInitials);
                    currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + dashInitials));

                    if (options.DotsUsed && !String.IsNullOrWhiteSpace(concatTo))
                    {
						// Add word without the dash, plus all permutations with the word next
						currentResults.Add(concatTo + "." + word.Trim('-'));
						currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + word.Trim('-')));

						// Add hyphenated initials, plus all permutations with the hyphenated initials next
						currentResults.Add(concatTo + "." + dashInitials);
						currentResults.AddRange(await ConcatenateWordsAsync(newRemainingWords, options, concatTo + "." + dashInitials));
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
