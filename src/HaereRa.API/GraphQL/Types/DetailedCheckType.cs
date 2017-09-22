using GraphQL.Types;
using HaereRa.API.Models;
using System.Collections.Generic;

namespace HaereRa.API.GraphQL.Types
{
    public class DetailedCheckType : ObjectGraphType<KeyValuePair<string, bool>>
    {
        public DetailedCheckType()
        {
            Field(x => x.Key).Description("The type of check performed.");
            Field(x => x.Value).Description("The result of the detailed check performed. Should return \"true\" in all cases.");
        }
    }
}
