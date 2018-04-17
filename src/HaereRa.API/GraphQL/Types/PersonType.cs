using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class PersonType : ObjectGraphType<Person>
    {
        public PersonType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a physical person within the company. This is ideally an employee ID however it could be any unique number.");
            Field(x => x.FullName).Description("The full name of the person (e.g. \"Jane Smith\", \"Nguyễn Tấn Dũng\" etc)");
            Field(x => x.KnownAs).Description("The shortened form that this person is normally referred to as in casual conversation (e.g. \"Jane\", \"Tấn Dũng\", etc)"); // TODO: Is it more casual to refer to a vietnamese name as Middle-Given, or just Given? Do I include the title here?
            Field(x => x.IsAdmin).Description("Indicates if this person has rights to edit the data on this site.");

            Field<ListGraphType<GroupMembershipType>>(nameof(Person.GroupMemberships), "The memberships that describe which groups this user belongs to and (optionally) manages.");
            Field<ListGraphType<ExternalAccountType>>(nameof(Person.ExternalAccounts), "The known and confirmed accounts found in third-party products.");
            Field<ListGraphType<ContactDetailType>>(nameof(Person.ContactDetails), "The specific contact address that this person uses on a specific communication platform.");
        }
    }
}
