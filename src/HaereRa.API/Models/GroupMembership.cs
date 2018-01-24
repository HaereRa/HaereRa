using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{Person.Name}: {Group.Name}")]
	public class GroupMembership
	{
		[Key]
		public int Id { get; set; }
        
		[Required]
		public int PersonId { get; set; }
		public Person Person { get; set; }
        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        [Required]
        public bool IsGroupManager { get; set; }
    }
}
