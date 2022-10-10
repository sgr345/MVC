using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.ViewModels
{
	public class UserInfo
	{
		[Required(ErrorMessage = "Please input your userid")]
		[MinLength(6, ErrorMessage = "Minimum 6")]
		[Display(Name = "UserID")]
		public string UserID { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Please input your password")]
		[MinLength(6, ErrorMessage = "Minimum 6")]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Please input your Name")]
		[Display(Name = "UserName")]
		public string UserName { get; set; }

		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Please input your Email")]
		[Display(Name = "UserEmail")]
		public string UserEmail { get; set; }

		public virtual ChangeInfo ChangeInfo { get; set; }
	}
}

