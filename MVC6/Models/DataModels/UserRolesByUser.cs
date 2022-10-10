using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.DataModels
{
	public class UserRolesByUser
	{
		[Key]
		public string UserID { get; set; }

		[Key]
		public string RoleID { get; set; } 

		public DateTime OwnedUtcDate { get; set; }

		public virtual User User { get; set; } 

		public virtual UserRole UserRole { get; set; } 
	}
}

