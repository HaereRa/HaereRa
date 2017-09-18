using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ExternalAccountType : ObjectGraphType<Profile>
    {
        public ExternalAccountType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a specific confirmed external account.");
            Field(x => x.ProfileAccountIdentifier).Description("The username of the external account on the external platform");

            Field<ExternalPlatformType>(nameof(Profile.ProfileType), "Which platform this account is located in.");
        }
    }
}
