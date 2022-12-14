using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.ViewModels
{
	public class LoginInfo
	{
		[Required(ErrorMessage = "Please input your userid")]
		[MinLength(6, ErrorMessage = "Minimum 6")]
		[Display(Name = "UserID")]
		public string UserID { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage ="Please input your password")]
		[MinLength(6, ErrorMessage = "Minimum 6")]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember")]
		public bool RememberMe { get; set; }
	}
}

