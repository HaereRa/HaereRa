using HaereRa.API.Models;
using System.Collections.Generic;

namespace HaereRa.API
{
	public class NotificationMessage
    {
		public Person Recipient { get; set; }
		public NotificationType NotificationType { get; set; }

		public IDictionary<Group, IEnumerable<Person>> ListOfUsersAffectedInGroupsManaged { get; set; }
		public IDictionary<ExternalPlatform, IEnumerable<Person>> ListOfUsersAffectedInPlatformsManaged { get; set; }

		public IEnumerable<Person> GetAffectedUsers()
		{
			// TODO: This is total garbage, and needs to be done properly using LINQ over nexted `foreach`es
			var listOfPeopleAffected = new List<Person>();

			foreach (var entry in ListOfUsersAffectedInGroupsManaged)
			{
				foreach (var person in entry.Value)
				{
					if (!listOfPeopleAffected.Exists(p => p.Id == person.Id))
					{
						listOfPeopleAffected.Add(person); 
					}
				}  
			}

			foreach (var entry in ListOfUsersAffectedInPlatformsManaged)
            {
				foreach (var person in entry.Value)
                {
                    if (!listOfPeopleAffected.Exists(p => p.Id == person.Id))
                    {
                        listOfPeopleAffected.Add(person);
                    }
                }
            }

			return listOfPeopleAffected;
		}
    }
}
