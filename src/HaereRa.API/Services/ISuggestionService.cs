using System.Collections.Generic;
using System.Threading.Tasks;

namespace HaereRa.API.Services
{
    public interface ISuggestionService
    {
        Task<List<string>> GetPossibleUsernamesAsync(string fullName, string knownAs, bool dashesAllowed = true, bool dotsUsed = false);
    }
}