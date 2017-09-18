using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ExternalPlatformEmailAlertType : ObjectGraphType<ProfileTypeEmailAlert>
    {
        public ExternalPlatformEmailAlertType()
        {
            Field(x => x.Id).Description("The unique id number assigned to an external platform alert.");
            Field(x => x.EmailAddress).Description("The email address to be alerted whenever a person with a external account on this platform changes state.");
        }
    }
}
