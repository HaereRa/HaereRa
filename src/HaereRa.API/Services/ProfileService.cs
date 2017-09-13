using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HaereRa.GraphQL.Services;
using HaereRa.GraphQL.Models;

namespace HaereRa.API.Services
{
    public class ProfileService : IProfileService
    {
        public ProfileService()
        {
        }

        public Task<IEnumerable<Profile>> GetAllProfilesForTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
