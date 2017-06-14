using System;
using HaereRa.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using HaereRa.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace HaereRa.API.Services
{
    public class PersonService : IPersonService
    {
        private readonly ISuggestionService _suggestionService;
        private readonly IProfileService _profileService;
        private readonly HaereRaDbContext _dbContext;

        public PersonService(HaereRaDbContext dbContext, IProfileService profileService, ISuggestionService suggestionService)
        {
            _dbContext = dbContext;
            _profileService = profileService;
            _suggestionService = suggestionService;
        }

        public async Task<Person> GetPersonAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
			var result = await _dbContext.People
				.Include(person => person.Department)
                    .ThenInclude(department => department.EmailAlerts)
				.Include(person => person.Profiles)
					.ThenInclude(profile => profile.ProfileType)
						.ThenInclude(profileType => profileType.EmailAlerts)
				.Include(person => person.ProfileSuggestions)
					.ThenInclude(profileSuggestion => profileSuggestion.ProfileType)
				.Where(p => p.Id == id)
				.SingleOrDefaultAsync();
            return result;
        }

        public async Task<IReadOnlyDictionary<int, IEnumerable<string>>> GetProfileSuggestionsAsync(Person person, List<ProfileType> profileTypes, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (person == null) throw new ArgumentNullException(nameof(person));
            if (profileTypes == null || !profileTypes.Any()) throw new ArgumentNullException(nameof(profileTypes));
            cancellationToken.ThrowIfCancellationRequested();

            var possibleUsernameMatches = await _suggestionService.GetPossibleUsernamesAsync(
                fullName: person.FullName,
                knownAs: person.KnownAs,
                dashesAllowed: false,
                dotsUsed: false, 
                cancellationToken: cancellationToken
            );

            var suggestionsList = new Dictionary<int, IEnumerable<string>>();
            foreach (var profileType in profileTypes)
            {
                var profileSuggestions = new List<string>();

                var allProfiles = await _profileService.GetAllProfilesForTypeAsync(profileType);
                foreach (var profile in allProfiles)
                {
                    var trimmedUsername = profile.ProfileAccountIdentifier
                        .Trim(new char[] { ' ', '_', '-', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' })
                        .ToLower();
                    if (possibleUsernameMatches.Contains(trimmedUsername))
                    {
                        profileSuggestions.Add(profile.ProfileAccountIdentifier);
                    }
                }

                if (profileSuggestions.Any())
                {
                    suggestionsList.Add(profileType.Id, profileSuggestions);
                }
            }

            return suggestionsList;
        }
    }


}
