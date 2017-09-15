using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Middleware.Services;
using GraphQL.Middleware.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace GraphQL.Middleware
{
    public class GraphQLMiddleware
    {
        public readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly GraphQLMiddlewareOptions _options;

        public GraphQLMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IOptions<GraphQLMiddlewareOptions> options)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _options = options.Value ?? new GraphQLMiddlewareOptions { RequestPath = "/GraphQL" };
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != "POST" || !context.Request.Path.Value.Equals(_options.RequestPath, StringComparison.Ordinal))
            {
                await _next(context);
                return;
            }

            var tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();

            var graphQLService = _serviceProvider.GetService<IGraphQLService>();

            // Read query parameters from body of request
            var query = ReadBody<GraphQLQuery>(context.Request.Body);

            // Execute query
            var result = await graphQLService.ExecuteQueryAsync(query.Query, query.Variables, query.OperationName, context.User, cancellationToken);

            var response = context.Response;

            if (result.Errors?.Count > 0)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.ContentType = "application/json";
                WriteResult(response.Body, result);
            }

            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "application/json";
            WriteResult(response.Body, result);
        }

        /// <summary>
        /// Read the input body and deserialise from JSON to <typeparamref name="TObject"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        private TObject ReadBody<TObject>(Stream input)
        {
            // TODO: We're assuming UTF-8 encoding here...
            using (var reader = new JsonTextReader(new StreamReader(input, Encoding.UTF8)))
            {
                reader.CloseInput = false;
                var jsonSerialiser = JsonSerializer.Create();
                return jsonSerialiser.Deserialize<TObject>(reader);
            }
        }

        /// <summary>
        /// Serialises the <typeparamref name="TObject"/> as JSON to the outut
        /// </summary>
        /// <param name="output"></param>
        /// <param name="body"></param>
        private void WriteResult<TObject>(Stream output, TObject body)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(output, Encoding.UTF8)))
            {
                writer.CloseOutput = false;

                var jsonSerialiser = JsonSerializer.Create();
                jsonSerialiser.Serialize(writer, body);
            }
        }
    }
}
