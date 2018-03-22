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

        Task UpdateSuggestionsAsync(int personId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false, CancellationToken cancellationToken = default);
        Task AddPossibleUsernameAsync(int personId, int profileTypeId, string username, CancellationToken cancellationToken = default);
        Task AcceptSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default);
        Task RejectSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default);
    }
}
