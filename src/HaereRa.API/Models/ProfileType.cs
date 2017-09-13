using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{Name}")]
	public class ProfileType
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string PluginAssembly { get; set; }
        [DataType(DataType.MultilineText)]
        public string PluginAssemblyOptions { get; set; }

		public List<Profile> Profiles { get; set; }
		public List<ProfileTypeEmailAlert> EmailAlerts { get; set; }
	}
}
