using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HaereRa.API.Services
{
    public interface ISuggestionService
    {
        Task<IEnumerable<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false, CancellationToken cancellationToken = default(CancellationToken));
    }
}