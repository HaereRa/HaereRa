using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{Name}")]
	public class Department
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }

		public List<Person> Members { get; set; }
		public List<DepartmentEmailAlert> EmailAlerts { get; set; }
	}
}
