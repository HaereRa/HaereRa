using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using HaereRa.API.DAL;
using HaereRa.API.Models;
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

        public async Task<Person> GetPersonAsync(int id, CancellationToken cancellationToken = default)
        {
			var result = await _dbContext.People
				.Include(person => person.GroupMemberships)
                    .ThenInclude(groupMembership => groupMembership.Group)
				.Include(person => person.ExternalAccounts)
					.ThenInclude(externalAccount => externalAccount.ExternalPlatform)
                .Include(person => person.ContactDetails)
                    .ThenInclude(contactDetail => contactDetail.NotificationProvider)
                .Where(p => p.Id == id)
				.SingleOrDefaultAsync();
            return result;
        }
    }


}
