using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
	public interface IExternalAccountService
	{
        Task<IEnumerable<ExternalAccount>> GetAllAccountsForPlatformAsync(int externalPlatformId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExternalAccount>> GetAllAccountsForPersonAsync(int personId, bool includePendingSuggestions = false, bool includeRejectedSuggestions = false, CancellationToken cancellationToken = default);
        Task<ExternalAccount> GetExternalAccountAsync(int externalAccountId, CancellationToken cancellationToken = default);
	}
}
