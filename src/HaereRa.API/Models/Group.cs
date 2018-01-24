using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{Name}")]
	public class Group
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }

		public List<GroupMembership> GroupMemberships { get; set; }
	}
}
