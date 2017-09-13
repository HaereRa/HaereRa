using System.ComponentModel.DataAnnotations;

namespace HaereRa.API.Models
{
    public class ProfileSuggestion
    {
		[Key]
		public int Id { get; set; }
		[Required]
		public string ProfileAccountIdentifier { get; set; } // TODO: This is a really crap name...
		[Required]
        public ProfileSuggestionStatus IsAccepted { get; set; }

		[Required]
		public int PersonId { get; set; }
		public Person Person { get; set; }
        [Required]
        public int ProfileTypeId { get; set; }
		public ProfileType ProfileType { get; set; }
    }
}
