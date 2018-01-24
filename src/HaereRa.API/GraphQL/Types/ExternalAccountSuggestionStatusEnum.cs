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

            AddValue(nameof(ExternalAccountSuggestionStatus.Accepted), "Confirmed as a matching external account for this person.", ExternalAccountSuggestionStatus.Accepted);
            AddValue(nameof(ExternalAccountSuggestionStatus.Pending), "Suggested as a possible external account for this person, but not confirmed.", ExternalAccountSuggestionStatus.Pending);
            AddValue(nameof(ExternalAccountSuggestionStatus.Rejected), "Marked as not a valid external account for this person.", ExternalAccountSuggestionStatus.Rejected);
        }
    }
}
