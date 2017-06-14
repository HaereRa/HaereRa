using GraphQL.Types;
using HaereRa.API.Models;
namespace HaereRa.API.GraphQL
{
    public class PersonType : ObjectGraphType<Person>
    {
        public PersonType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a physical person within the company. This is ideally an employee ID however it could be any unique number.");
            Field(x => x.FullName).Description("The full name of the person (e.g. \"Jane Smith\", \"Nguyễn Tấn Dũng\" etc)");
            Field(x => x.KnownAs).Description("The shortened form that this person is normally referred to as in casual conversation (e.g. \"Jane\", \"Tấn Dũng\", etc)"); // TODO: Is it more casual to refer to a vietnamese name as Middle-Given, or just Given? Do I include the title here?
            Field(x => x.IsAdmin).Description("Indicates if this person has rights to edit the data on this site.");

            Field<DepartmentType>(nameof(Person.Department), "The department this person belongs to.");
            Field<ListGraphType<ProfileType>>(nameof(Person.Profiles), "The known and confirmed profiles found in third-party products.");
            Field<ListGraphType<ProfileSuggestionType>>(nameof(Person.ProfileSuggestions), "The discovered possible profiles found in third-party products that might match this person.");
        }
    }

    public class ProfileType : ObjectGraphType<Profile>
	{
		public ProfileType()
		{
			Field(x => x.Id).Description("The unique id number assigned to a specific confirmed profile.");
            Field(x => x.ProfileAccountIdentifier).Description("The username of the profile on the third-party service");

			Field<ProfileTypeType>(nameof(Profile.ProfileType), "Which service this profile is located in.");
		}
	}

	public class ProfileSuggestionType : ObjectGraphType<ProfileSuggestion>
	{
		public ProfileSuggestionType()
		{
			Field(x => x.Id).Description("The unique id number assigned to a specific suggested profile.");
			Field(x => x.ProfileAccountIdentifier).Description("The username of the profile on the third-party service");
            Field<ProfileSuggestionStatusEnum>(nameof(ProfileSuggestion.IsAccepted), "The username of the profile on the third-party service");

			Field<ProfileTypeType>(nameof(Profile.ProfileType), "Which service this profile is located in.");
		}
	}

	public class ProfileSuggestionStatusEnum : EnumerationGraphType
	{
		public ProfileSuggestionStatusEnum()
		{
			Name = "ProfileSuggestionStatus";
			Description = "Describes whether or not this possible suggestion was confirmed as a valid finding or not.";

            AddValue(nameof(ProfileSuggestionStatus.Accepted), "Confirmed as a matching profile for this person.", ProfileSuggestionStatus.Accepted);
            AddValue(nameof(ProfileSuggestionStatus.Pending), "Suggested as a possible profile for this person, but not confirmed.", ProfileSuggestionStatus.Pending);
            AddValue(nameof(ProfileSuggestionStatus.Rejected), "Marked as not a valid profile for this person.", ProfileSuggestionStatus.Rejected);
		}
	}

    public class ProfileTypeType : ObjectGraphType<HaereRa.API.Models.ProfileType>
	{
		public ProfileTypeType()
		{
			Field(x => x.Id).Description("The unique id number assigned to a profile type.");
            Field(x => x.Name).Description("The friendly display name for the profile type.");
            Field(x => x.PluginAssembly).Description("The techncial name of the assembly housing the Profile Plugin."); 

            Field<ListGraphType<ProfileTypeEmailAlertType>>(nameof(HaereRa.API.Models.ProfileType.EmailAlerts), "The email addresses to be alerted whenever a person with a profile of this type changes state.");
		}
	}

    public class ProfileTypeEmailAlertType : ObjectGraphType<ProfileTypeEmailAlert>
    {
        public ProfileTypeEmailAlertType()
        {
            Field(x => x.Id).Description("The unique id number assigned to a profile type alert.");
            Field(x => x.EmailAddress).Description("The email address to be alerted whenever a person with a profile of this type changes state.");
        }
    }

    public class DepartmentType : ObjectGraphType<Department>
	{
		public DepartmentType()
		{
			Field(x => x.Id).Description("The unique id number assigned to a department.");
            Field(x => x.Name).Description("The display name of the department.");

            Field<ListGraphType<DepartmentEmailAlertType>>(nameof(Department.EmailAlerts), "The email addresses to be alerted whenever a person in this department changes state.");
		}
	}

	public class DepartmentEmailAlertType : ObjectGraphType<DepartmentEmailAlert>
	{
		public DepartmentEmailAlertType()
		{
			Field(x => x.Id).Description("The unique id number assigned to a department alert.");
			Field(x => x.EmailAddress).Description("The email address to be alerted whenever a person in this department changes state.");
		}
	}
}
