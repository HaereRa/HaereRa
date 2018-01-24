using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ExternalAccountType : ObjectGraphType<ExternalAccount>
    {
        public ExternalAccountType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a specific confirmed external account.");
            Field(x => x.ExternalAccountIdentifier).Description("The username of the external account on the external platform");
            Field<ExternalAccountSuggestionStatusEnum>(nameof(ExternalAccount.IsSuggestionAccepted), "The username of the profile on the third-party service");

            Field<ExternalPlatformType>(nameof(ExternalAccount.ExternalPlatform), "Which platform this account is located in.");
        }
    }
}
