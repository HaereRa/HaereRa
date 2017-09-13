using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.GraphQL.Models;

namespace HaereRa.GraphQL.Services
{
    public interface IPersonService
    {
        Task<Person> GetPersonAsync(int id, CancellationToken cancellationToken = default(CancellationToken));
    }
}