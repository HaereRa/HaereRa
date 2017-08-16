using HaereRa.API.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using HaereRa.API.DAL;
using Microsoft.EntityFrameworkCore;

namespace HaereRa.API.Services
{
    public class PersonService : IPersonService
    {
        private readonly HaereRaDbContext _dbContext;

        public PersonService(HaereRaDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }


}
