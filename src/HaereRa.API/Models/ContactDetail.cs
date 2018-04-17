using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
    [DebuggerDisplay("{NotificationProvider.Name}: {ContactDetailIdentifier}")]
    public class ContactDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ContactDetailIdentifier { get; set; }

        [Required]
        public int NotificationProviderId { get; set; }
        public NotificationProvider NotificationProvider { get; set; }
        [Required]
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}