using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HaereRa.API.Models
{
    [DebuggerDisplay("{Name}")]
    public class NotificationProvider
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string PluginAssembly { get; set; }
        [DataType(DataType.MultilineText)]
        public string PluginAssemblyOptions { get; set; }

        public List<ContactDetail> NotificationAccounts { get; set; }
    }
}
