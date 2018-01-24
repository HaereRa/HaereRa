using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class GroupMembershipType : ObjectGraphType<GroupMembership>
    {
        public GroupMembershipType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a group.");

            Field<PersonType>(nameof(GroupMembership.Person), "The person this membership relates to.");
            Field<GroupType>(nameof(GroupMembership.Group), "The group this member belongs to.");

            Field(x => x.IsGroupManager).Description("Describes if the person manages the group they belong to. If \"true\", this user is also a manager of the group and will recieve alerts about group changes.");
        }
    }
}
