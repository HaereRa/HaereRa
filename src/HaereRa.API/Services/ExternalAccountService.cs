using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.DAL;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
    public class ExternalAccountService : IExternalAccountService
    {
        private readonly HaereRaDbContext _dbContext;

        public ExternalAccountService(HaereRaDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
