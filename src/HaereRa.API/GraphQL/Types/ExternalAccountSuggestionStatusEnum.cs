using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ExternalAccountSuggestionStatusEnum : EnumerationGraphType
    {
        public ExternalAccountSuggestionStatusEnum()
        {
            Name = "ExternalAccountSuggestionStatus";
            Description = "Describes if this suggested external account was confirmed as a valid match for the person or not.";

            AddValue(nameof(ProfileSuggestionStatus.Accepted), "Confirmed as a matching external account for this person.", ProfileSuggestionStatus.Accepted);
            AddValue(nameof(ProfileSuggestionStatus.Pending), "Suggested as a possible external account for this person, but not confirmed.", ProfileSuggestionStatus.Pending);
            AddValue(nameof(ProfileSuggestionStatus.Rejected), "Marked as not a valid external account for this person.", ProfileSuggestionStatus.Rejected);
        }
    }
}
