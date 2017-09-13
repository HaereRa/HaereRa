using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.GraphQL.Models
{
	[DebuggerDisplay("{DateTime}: {Status.ToString()}")]
	public class PersonStatus
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public PersonStatusType Status { get; set; }
		[Required]
		[DataType(DataType.Date)]
		public DateTimeOffset DateTime { get; set; }
		[DataType(DataType.MultilineText)]
		public string Notes { get; set; }

		[Required]
		public int PersonId { get; set; }
		public Person Person { get; set; }
	}
}
