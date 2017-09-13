using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HaereRa.GraphQL.Models;

namespace HaereRa.GraphQL.Services
{
	public interface IProfileService
	{
        Task<IEnumerable<Profile>> GetAllProfilesForTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default(CancellationToken));
	}
}
