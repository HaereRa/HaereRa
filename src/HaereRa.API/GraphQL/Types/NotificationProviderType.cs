using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class NotificationProviderType : ObjectGraphType<NotificationProvider>
    {
        public NotificationProviderType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a notification provider.");
            Field(x => x.Name).Description("The friendly display name for the notification provider.");
            Field(x => x.PluginAssembly).Description("The techncial name of the assembly housing the Notification Provider Plugin.");
        }
    }
}
