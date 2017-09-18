using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;

namespace GraphQL.Middleware.Services
{
    public interface IGraphQLService
    {
        // TODO: ExecutionResult is a leaky abstraction, should be a string
        Task<ExecutionResult> ExecuteQueryAsync(string query, string variables, string operationName, ClaimsPrincipal userContext = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}