using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.DataModels
{
	public class User
	{
		[Key]
		public string UserID { get; set; }

		public string UserName { get; set; }

		public string UserEmail { get; set; }

		public string GUIDSalt { get; set; }

		public string RNGSalt { get; set; }

		public string PasswordHash { get; set; }

		public int AccessFailedCount { get; set; }

		public bool IsMembershipWithdrawn { get; set; }

		public DateTime JoinedUtcDate { get; set; }

		public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
	}
}

