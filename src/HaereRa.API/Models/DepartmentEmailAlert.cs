using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{EmailAddress} ({Department.Name})")]
	public class DepartmentEmailAlert
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }

		[Required]
		public int DepartmentId { get; set; }
		public Department Department { get; set; }
	}
}
