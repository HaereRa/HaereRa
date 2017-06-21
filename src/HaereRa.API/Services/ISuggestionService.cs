using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
    public interface ISuggestionService
    {
        Task<IEnumerable<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false, CancellationToken cancellationToken = default(CancellationToken));
        Task AcceptSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default(CancellationToken));
        Task RejectSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default(CancellationToken));
        Task<ProfileSuggestion> GetSuggestionAsync(int suggestionId, CancellationToken cancellationToken = default(CancellationToken));
    }
}