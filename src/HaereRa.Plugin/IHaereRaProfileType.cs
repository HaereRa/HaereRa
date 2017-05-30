using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HaereRa.Plugin
{
    public interface IHaereRaProfileType
    {
        Task<List<string>> ListProfilesAsync();
        Task<ProfileTypeListing> GetProfileAsync(string accountIdentifier);
        Task DeactivateProfileAsync(string accountIdentifier);
        Task ActivateProfileAsync(string accountIdentifier);
    }
}
