using System;
using System.Threading;
using System.Threading.Tasks;

namespace HaereRa.Plugin
{
    public interface IEnableDisableExternalUserAccounts
    {
        Task DeactivateProfileAsync(string accountIdentifier, CancellationToken cancellationToken = default(CancellationToken));
        Task ActivateProfileAsync(string accountIdentifier, CancellationToken cancellationToken = default(CancellationToken));
    }
}
