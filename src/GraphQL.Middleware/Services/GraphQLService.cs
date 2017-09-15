using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphQL.Types;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace GraphQL.Middleware.Services
{
    public class GraphQLService : IGraphQLService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISchema _schema;

        public GraphQLService(IHostingEnvironment hostingEnvironment, ISchema schema)
        {
            _hostingEnvironment = hostingEnvironment;
            _schema = schema;
        }

        public async Task<ExecutionResult> ExecuteQueryAsync(string query, string variables, string operationName, ClaimsPrincipal userContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!String.IsNullOrWhiteSpace(query)) throw new ArgumentNullException(nameof(query));

                var start = DateTime.UtcNow;

                var result = await new DocumentExecuter().ExecuteAsync(_ =>
                {
                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                    _.Schema = _schema;
                    _.Query = query;
                    _.Inputs = variables?.ToInputs();
                    _.OperationName = operationName;
                    _.UserContext = userContext;
                    _.ExposeExceptions = _hostingEnvironment.IsDevelopment();
                    _.CancellationToken = cancellationToken;
                }).ConfigureAwait(false);

                var report = StatsReport.From(_schema, result.Operation, result.Perf, start); // TODO: Actually include this

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
