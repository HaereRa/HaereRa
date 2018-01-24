using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ExternalPlatformType : ObjectGraphType<ExternalPlatform>
    {
        public ExternalPlatformType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a profile type.");
            Field(x => x.Name).Description("The friendly display name for the profile type.");
            Field(x => x.PluginAssembly).Description("The techncial name of the assembly housing the Profile Plugin.");
        }
    }
}
