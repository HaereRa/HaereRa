using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using HaereRa.Plugin.Models;

namespace HaereRa.Plugin
{
    public interface IFetchExternalUserAccounts
    {
        Task<List<ExternalUserAccount>> ListProfilesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<ExternalUserAccount> GetProfileAsync(string accountIdentifier, CancellationToken cancellationToken = default(CancellationToken));
    }
}
