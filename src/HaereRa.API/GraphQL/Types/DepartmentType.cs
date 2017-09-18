using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL.Types
{
    public class DepartmentType : ObjectGraphType<Department>
    {
        public DepartmentType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a department.");
            Field(x => x.Name).Description("The display name of the department.");

            Field<ListGraphType<DepartmentEmailAlertType>>(nameof(Department.EmailAlerts), "The email addresses to be alerted whenever a person in this department changes state.");
        }
    }
}
