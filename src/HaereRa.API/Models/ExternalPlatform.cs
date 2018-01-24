using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{Name}")]
	public class ExternalPlatform
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string PluginAssembly { get; set; }
        [DataType(DataType.MultilineText)]
        public string PluginAssemblyOptions { get; set; }

		public List<ExternalAccount> ExternalAccounts { get; set; }
	}
}
