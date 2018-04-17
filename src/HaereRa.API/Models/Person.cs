using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
    [DebuggerDisplay("{FullName}")]
    public class Person
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
		public string KnownAs { get; set; }
        public bool IsAdmin { get; set; }
        
        public List<GroupMembership> GroupMemberships { get; set; }
        public List<ExternalAccount> ExternalAccounts { get; set; }
        public List<ContactDetail> ContactDetails { get; set; }
    }
}
