using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace HaereRa.API.Models
{
	[DebuggerDisplay("{EmailAddress} ({ProfileType.Name})")]
	public class ProfileTypeEmailAlert
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[EmailAddress]
		public string EmailAddress { get; set; }

		[Required]
		public int ProfileTypeId { get; set; }
		public ProfileType ProfileType { get; set; }
	}
}
