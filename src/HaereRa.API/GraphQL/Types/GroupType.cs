using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class GroupType : ObjectGraphType<Group>
    {
        public GroupType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a group.");
            Field(x => x.Name).Description("The display name of the group.");

            Field<ListGraphType<GroupMembershipType>>(nameof(Group.GroupMemberships), "The email addresses to be alerted whenever a person in this department changes state.");
        }
    }
}
