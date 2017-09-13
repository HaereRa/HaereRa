using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
	public interface IProfileService
	{
        Task<IEnumerable<Profile>> GetAllProfilesForTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default(CancellationToken));
	}
}
