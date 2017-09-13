using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using HaereRa.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using HaereRa.API.GraphQL;
using GraphQL;
using GraphQL.Instrumentation;
using System.Threading;

namespace HaereRa.API.Controllers
{
    [Route("[controller]")]
    public class GraphQLController : Controller
    {
        private readonly HaereRaQuery _haereRaQuery;
        private readonly HaereRaMutation _haereRaMutation;

        public GraphQLController(HaereRaQuery haereRaQuery, HaereRaMutation haereRaMutation)
        {
            _haereRaQuery = haereRaQuery;
            _haereRaMutation = haereRaMutation;
        }

        // TODO: We're only handling a small slice of the recommended HTTP 
        // delivery methods. We should fix this: http://graphql.org/learn/serving-over-http/

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)  // TODO: This document binder is *super* picky, results in lots of `query = null`
        {
            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();

            if (query == null)
            {
                return BadRequest(
                    "{\n  errors: [\n    { message: 'Malformed JSON request object. Double check the JSON is compliant with the GraphQL spec.' }\n  ]\n}"
                );
            }

            var schema = new Schema { 
                Query = _haereRaQuery, 
                Mutation = _haereRaMutation,
            };

            var start = DateTime.UtcNow;

            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                _.Schema = schema;
                _.Query = query.Query;
                _.Inputs = query.Variables?.ToInputs();
                _.CancellationToken = cancellationToken;
            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result.Errors.First().Message);
            }

            var report = StatsReport.From(schema, result.Operation, result.Perf, start);

            return Ok(result);
        }
    }
}
