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

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public List<PersonStatus> StatusHistory { get; set; }
        public List<Profile> Profiles { get; set; }
        public List<ProfileSuggestion> ProfileSuggestions { get; set; }
    }
}
