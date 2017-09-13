using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

using HaereRa.GraphQL;

namespace Microsoft.AspNetCore.Builder
{
    public static class HaereRaGraphQLExtensions
    {
        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<HaereRaGraphQLMiddleware>();
        }

        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder, string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath))
                throw new ArgumentNullException(nameof(requestPath));

            return builder
                .UseGraphQL(new HaereRaGraphQLOptions { RequestPath = requestPath });
        }

        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder, HaereRaGraphQLOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return builder
                .UseMiddleware<HaereRaGraphQLMiddleware>(Options.Create(options));
        }
    }
}
