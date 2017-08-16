using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
    public interface IPersonService
    {
        Task<Person> GetPersonAsync(int id, CancellationToken cancellationToken = default(CancellationToken));
    }
}