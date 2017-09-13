using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{ProfileType.Name}: {ProfileAccountIdentifier}")]
	public class Profile
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string ProfileAccountIdentifier { get; set; } // TODO: This is a really crap name...

		[Required]
		public int ProfileTypeId { get; set; }
		public ProfileType ProfileType { get; set; }
		[Required]
		public int PersonId { get; set; }
		public Person Person { get; set; }
	}
}
