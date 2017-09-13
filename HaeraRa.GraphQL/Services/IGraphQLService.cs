using System.Threading;
using System.Threading.Tasks;
using GraphQL;

namespace HaereRa.GraphQL.Services
{
    public interface IGraphQLService
    {
        // TODO: ExecutionResult is a leaky abstraction, should be a string
        Task<ExecutionResult> ExecuteQueryAsync(string query, string variables, CancellationToken cancellationToken = default(CancellationToken));
    }
}