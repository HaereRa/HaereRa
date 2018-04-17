using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class ContactDetailType : ObjectGraphType<ContactDetail>
    {
        public ContactDetailType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a specific confirmed external account.");
            Field(x => x.ContactDetailIdentifier).Description("The username of the external account on the external platform");

            Field<PersonType>(nameof(ContactDetail.Person), "Which person this contact address belongs to.");
            Field<NotificationProviderType>(nameof(ContactDetail.NotificationProvider), "Which platform used to contact this address on.");
        }
    }
}
