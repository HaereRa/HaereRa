using GraphQL.Types;
using HaereRa.API.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HaereRa.API.GraphQL.Types
{
    public class HealthCheckType : ObjectGraphType<HealthCheck>
    {
        public HealthCheckType()
        {
            Field<BooleanGraphType>(nameof(HealthCheck.Basic), "The status of the service as a basic check. Should return \"true\" in all cases.");
            Field<ListGraphType<DetailedCheckType>>(nameof(HealthCheck.Detailed), "The status of each of the more comprehensive checks on the system. Include this status sparingly.");
        }
    }
}
