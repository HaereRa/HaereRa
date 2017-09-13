using System;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using HaereRa.API.GraphQL;
using GraphQL;
using GraphQL.Types;
using GraphQL.Instrumentation;
using Microsoft.AspNetCore.Hosting;

namespace HaereRa.API
{
    public class GraphQLService : IGraphQLService
    {
        private readonly HaereRaQuery _haereRaQuery;
        private readonly HaereRaMutation _haereRaMutation;
        private readonly IHostingEnvironment _hostingEnvironment;

        public GraphQLService(IHostingEnvironment hostingEnvironment, HaereRaQuery haereRaQuery, HaereRaMutation haereRaMutation)
        {
            _hostingEnvironment = hostingEnvironment;
            _haereRaQuery = haereRaQuery;
            _haereRaMutation = haereRaMutation;
        }

        public async Task<ExecutionResult> ExecuteQueryAsync(string query, string variables, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!String.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));

                var schema = new Schema
                {
                    Query = _haereRaQuery,
                    Mutation = _haereRaMutation,
                };

                var start = DateTime.UtcNow;

                var result = await new DocumentExecuter().ExecuteAsync(_ =>
                {
                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                    _.Schema = schema;
                    _.Query = query;
                    _.Inputs = variables?.ToInputs();
                    _.CancellationToken = cancellationToken;
                }).ConfigureAwait(false);

                var report = StatsReport.From(schema, result.Operation, result.Perf, start); // TODO: Actually include this

                if (_hostingEnvironment.IsDevelopment()) // TODO: Include if admin user
                {
                    result.ExposeExceptions = true;
                }
                else
                {
                    result.ExposeExceptions = false;
                }

                return result;
            }
            catch (AggregateException aex)
            {
                var executionErrors = new ExecutionErrors();
                foreach (var ex in aex.Flatten().InnerExceptions)
                {
                    executionErrors.Add(new ExecutionError(ex.Message, ex));
                }
                return new ExecutionResult
                {
                    Errors = executionErrors,
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Errors = new ExecutionErrors
                    {
                        new ExecutionError(ex.Message, ex),
                    },
                };
            }
        }
    }
}
