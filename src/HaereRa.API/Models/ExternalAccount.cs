using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{ExternalPlatform.Name}: {ExternalAccountIdentifier}")]
	public class ExternalAccount
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string ExternalAccountIdentifier { get; set; }

		[Required]
		public int ExternalPlatformId { get; set; }
		public ExternalPlatform ExternalPlatform { get; set; }
		[Required]
		public int PersonId { get; set; }
		public Person Person { get; set; }

        [Required]
        public bool IsPlatformManager { get; set; }
        [Required]
        public ExternalAccountSuggestionStatus IsSuggestionAccepted { get; set; }
    }
}
