using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ExternalAccountSuggestionType : ObjectGraphType<ProfileSuggestion>
    {
        public ExternalAccountSuggestionType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a specific suggested profile.");
            Field(x => x.ProfileAccountIdentifier).Description("The username of the profile on the third-party service");
            Field<ExternalAccountSuggestionStatusEnum>(nameof(ProfileSuggestion.IsAccepted), "The username of the profile on the third-party service");

            Field<ExternalPlatformType>(nameof(Profile.ProfileType), "Which service this profile is located in.");
        }
    }
}
