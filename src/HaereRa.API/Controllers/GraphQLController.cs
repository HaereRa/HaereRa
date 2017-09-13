using System.Linq;
using System.Threading.Tasks;
using HaereRa.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using System.Threading;

namespace HaereRa.API.Controllers
{
    [Route("[controller]")]
    public class GraphQLController : Controller
    {
        private readonly IGraphQLService _graphQLService;

        public GraphQLController(IGraphQLService graphQLService)
        {
            _graphQLService = graphQLService;
        }

        // TODO: We're only handling a small slice of the recommended HTTP 
        // delivery methods. We should fix this: http://graphql.org/learn/serving-over-http/
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)  // TODO: This document binder is *super* picky, results in lots of `query = null`
        {
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _graphQLService.ExecuteQueryAsync(query.Query, query.Variables, cancellationToken);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
