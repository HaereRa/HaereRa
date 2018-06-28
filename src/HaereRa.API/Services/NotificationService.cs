using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HaereRa.API.DAL;
using HaereRa.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace HaereRa.API.Services
{
	public class NotificationService : INotificationService
	{
		private readonly HaereRaDbContext _dbContext;

		public NotificationService(HaereRaDbContext dbContext)
		{
			_dbContext = dbContext;
		}
        
		public async Task<IEnumerable<NotificationMessage>> GetNotificationMessagesAsync(IEnumerable<int> affectedUserIds, NotificationType notificationType, CancellationToken cancellationToken = default)
		{
			if (affectedUserIds == null || !affectedUserIds.Any()) throw new ArgumentNullException(nameof(affectedUserIds));
            
			var messages = new List<NotificationMessage>();
            
			IDictionary<Group, IEnumerable<Person>> groupsAffected = new Dictionary<Group, IEnumerable<Person>>();
			IDictionary<ExternalPlatform, IEnumerable<Person>> platformsAffected = new Dictionary<ExternalPlatform, IEnumerable<Person>>();

			foreach (var affectedUserId in affectedUserIds)
			{
				var affectedUser = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == affectedUserId);
				if (affectedUser == null) throw new KeyNotFoundException($"User Id \"{affectedUserId.ToString()}\" does not exist in the database.");

				var groupsForUser = await _dbContext.GroupMemberships.Where(gm => gm.PersonId == affectedUserId).Select(m => m.Group).ToListAsync(cancellationToken);
				var platformsForUser = await _dbContext.ExternalAccounts.Where(gm => gm.PersonId == affectedUserId).Select(m => m.ExternalPlatform).ToListAsync(cancellationToken);

				foreach (var group in groupsForUser)
				{
					if (!groupsAffected.Any(ga => ga.Key.Id == group.Id))
					{
						// The group doesn't yet exist, create it and add the user as the first entry
						groupsAffected.Add(group, new List<Person> { affectedUser });
					}
					else
					{
						groupsAffected // From the list of all affected groups
							.FirstOrDefault(ga => ga.Key.Id == group.Id) // Get the group for this loop iteration
							.Value // and for the list of people affected in this group
							.Add(affectedUser); // add this user to it
					}
				}

				foreach (var platform in platformsForUser)
				{
					if (!platformsAffected.Any(pa => pa.Key.Id == platform.Id))
					{
						// The platform doesn't yet exist, create it and add the user as the first entry
						platformsAffected.Add(platform, new List<Person> { affectedUser });
					}
					else
					{
						platformsAffected // From the list of all affected external platforms
							.FirstOrDefault(pa => pa.Key.Id == platform.Id) // Get the platform for this loop iteration
							.Value // and for the list of people affected in this platform
							.Add(affectedUser); // add this user to it
					}
				}

				//var managersToContact = new List<Person>();

				//managersToContact.AddRange(
				//	await _dbContext.GroupMemberships // On the GroupMemberships table
				//	.Include(gm => gm.Person) // And the Person table
				//	.Where(gm =>
				//		groupsForUser.Any(g => g.Id == gm.GroupId) && // Where the group membership is for one of the above list of groups
				//		gm.IsGroupManager // And the membership designates them as a group manager
				//	)
				//	.Select(gm => gm.Person) // Return the person from the membership
				//	.ToListAsync() // As a list
				//);

				//managersToContact.AddRange(
				//    await _dbContext.ExternalAccounts // On the ExternalAccounts table
				//    .Include(ea => ea.Person) // And the Person table
				//    .Where(ea =>
				//        platformsForUser.Any(p => p.Id == ea.ExternalPlatformId) && // Where the external account is for one of the above list of platforms
				//        ea.IsPlatformManager // And the account designates them as a platform manager
				//    )
				//    .Select(ea => ea.Person) // Return the person from the account
				//    .ToListAsync() // As a list
				//);
			}

			foreach (var groupAffected in groupsAffected)
			{
				var managersInGroupToContact = new List<Person>();

				managersInGroupToContact.AddRange(
				  await _dbContext.GroupMemberships // On the GroupMemberships table
				  .Include(gm => gm.Person) // And the Person table
				  .Where(gm =>
					  gm.GroupId == groupAffected.Key.Id && // Where the group membership is for this group
					  gm.IsGroupManager // And the membership designates them as a group manager
				  )
				  .Select(gm => gm.Person) // Return the person from each membership
				  .ToListAsync() // As a list
				);

				foreach (var manager in managersInGroupToContact)
				{
					if (!messages.Any(m => m.Recipient.Id == manager.Id))
					{
						// The manager doesn't yet exist, create a new message it and add the group to it
						messages.Add(new NotificationMessage
						{
							Recipient = manager,
							NotificationType = notificationType,
							ListOfUsersAffectedInGroupsManaged = new Dictionary<Group, IEnumerable<Person>> { { groupAffected.Key, groupAffected.Value } },
							ListOfUsersAffectedInPlatformsManaged = new Dictionary<ExternalPlatform, IEnumerable<Person>>()
						});
					}
					else
					{
						platformsAffected // From the list of all affected external platforms
							.FirstOrDefault(pa => pa.Key.Id == platform.Id) // Get the platform for this loop iteration
							.Value // and for the list of people affected in this platform
							.Add(affectedUser); // add this user to it
					}
				}
			}
		}

        public async Task SendNotificationsAsync(NotificationMessage notificationMessage, CancellationToken cancellationToken = default)
        {
        	await SendNotificationsAsync(new List<NotificationMessage> { notificationMessage }, cancellationToken);
        }

        public Task SendNotificationsAsync(IEnumerable<NotificationMessage> notificationMessages, CancellationToken cancellationToken = default)
        {
        	throw new NotImplementedException();
        }
    }
}

  //      public async Task NotifyAsync(IEnumerable<int> affectedUserIds, NotificationType notificationType, CancellationToken cancellationToken = default)
  //      {
		//	if (affectedUserIds == null || !affectedUserIds.Any()) throw new ArgumentNullException(nameof(affectedUserIds));
            
		//	var groupIdsForAffectedUserIds = new Dictionary<int, List<int>>(); // Key = Group, Value = List<Person>
		//	var platformIdsForAffectedUserIds = new Dictionary<int, List<int>>(); // Key = ExternalPlatform, Value = List<Person>
            
		//	// Get Dict<Group, List<User>>, where keys are list of groups these users are in, and which users from affectedUsers are in it
  //          // Get Dict<ExternalPlatforms, List<User>>, where keys are list of external platforms these users are in, and which users from affectedUsers are in it
  //          // Neither of the above should have groups/external platforms with an empty list of users

		//	// TODO: Might be ways to optimise this, it's pretty chatty right now
		//	foreach (var affectedUserId in affectedUserIds)
  //          {
  //              // Get list of (unique) groups for this user
		//		var groupsForUser = await _dbContext.GroupMemberships.Where(gm => gm.PersonId == affectedUserId).Select(m => m.Group).ToListAsync(cancellationToken);

		//		foreach (var group in groupsForUser) 
		//		{
		//			if (!groupIdsForAffectedUserIds.ContainsKey(group.Id))
		//			{
		//				groupIdsForAffectedUserIds.Add(group.Id, new List<int>());
		//			}
		//			if (!groupIdsForAffectedUserIds[group.Id].Contains(affectedUserId))
		//			{
		//				groupIdsForAffectedUserIds[group.Id].Add(affectedUserId);
		//			}               
		//		}

		//		var platformsForUser = await _dbContext.ExternalAccounts.Where(gm => gm.PersonId == affectedUserId).Select(m => m.ExternalPlatform).ToListAsync(cancellationToken);

		//		foreach (var platform in platformsForUser)
  //              {
		//			if (!platformIdsForAffectedUserIds.ContainsKey(platform.Id))
  //                  {
		//				platformIdsForAffectedUserIds.Add(platform.Id, new List<int>());
  //                  }
		//			if (!platformIdsForAffectedUserIds[platform.Id].Contains(affectedUserId))
  //                  {
		//				platformIdsForAffectedUserIds[platform.Id].Add(affectedUserId);
  //                  }
  //              }            
  //          }
            
  //          // Pass group dict to NotifyGroup         
		//	foreach(var groupEntry in groupIdsForAffectedUserIds)
		//	{
		//		await NotifyGroup(groupEntry.Key, groupEntry.Value, notificationType, cancellationToken);
		//	}

		//	// Pass platform dict to NotifyPlatform
		//	foreach (var platformEntry in platformIdsForAffectedUserIds)
  //          {
		//		await NotifyPlatform(platformEntry.Key, platformEntry.Value, notificationType, cancellationToken);
  //          }
  //      }

		//public Task NotifyGroup(int groupId, IEnumerable<int> affectedUserIds, NotificationType notificationType, CancellationToken cancellationToken = default)
  //      {
		//	// TODO: Issue here: managers will get one notification PER group (or platform) managed. This should be condensed into a single message per manager, as per NotificationMessage.cs

  //          // Get list of managers
		//	var managersInGroup = _dbContext.

  //          // foreach manager
  //          //     pass affectedUsers to NotifyPerson

  //          throw new NotImplementedException();
  //      }
      
		//public Task NotifyPlatform(int externalPlatformId, IEnumerable<int> affectedUserIds, NotificationType notificationType, CancellationToken cancellationToken = default)
  //      {
  //          // Get list of managers

  //          // foreach manager
  //          //     pass affectedUsers to NotifyPerson

  //          throw new NotImplementedException();
  //      }

		//public Task NotifyPerson(int personId, IEnumerable<int> affectedUserIds, NotificationType notificationType, CancellationToken cancellationToken = default)
        //{
        //    // Get list of contact details for person
        //    // foreach contactDetail.NotificationProvider
        //    //     Call NotificationProviderPlugin.SendMessage(person, NotificationType, affectedusers[])

        //    throw new NotImplementedException();
        //}
