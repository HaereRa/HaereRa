using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace GraphQL.Middleware
{
    public static class GraphQLExtensions
    {
        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<GraphQLMiddleware>();
        }

        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder, string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                throw new ArgumentNullException(nameof(requestPath));

            return builder
                .UseGraphQL(new GraphQLMiddlewareOptions { RequestPath = requestPath });
        }

        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder, GraphQLMiddlewareOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return builder
                .UseMiddleware<GraphQLMiddleware>(Options.Create(options));
        }
    }
}
