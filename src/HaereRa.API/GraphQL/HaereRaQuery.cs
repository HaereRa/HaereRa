using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using HaereRa.API.GraphQL.Types;
using HaereRa.API.Services;
using System.Collections.Generic;

namespace HaereRa.API.GraphQL
{
    public class HaereRaQuery : ObjectGraphType
    {
        public HaereRaQuery(IHttpContextAccessor httpContextAccessor, IPersonService personService)
        {
            var user = httpContextAccessor.HttpContext.User;

            Field<HealthCheckType>(
                name: "health",
                description: "Perform a health check.",
                resolve: context =>
                {
                    return new HealthCheck
                    {
                        Basic = true,
                        Detailed = null
                    };
                }
            );

            Field<PersonType>(
                "person",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "id" }
                ),
                description: "A physical person in the organisation.",

                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return personService.GetPersonAsync(id);
                }
            );
        }
    }
}
