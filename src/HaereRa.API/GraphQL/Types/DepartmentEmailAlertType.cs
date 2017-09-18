using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class DepartmentEmailAlertType : ObjectGraphType<DepartmentEmailAlert>
    {
        public DepartmentEmailAlertType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a department alert.");
            Field(x => x.EmailAddress).Description("The email address to be alerted whenever a person in this department changes state.");
        }
    }
}
