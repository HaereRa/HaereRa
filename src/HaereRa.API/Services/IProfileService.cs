using System.Threading.Tasks;
using System.Collections.Generic;
using HaereRa.API.Models;
using System.Threading;

namespace HaereRa.API.Services
{
	public interface IProfileService
	{
        Task<IEnumerable<Profile>> GetAllProfilesForTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default(CancellationToken));
	}
}
