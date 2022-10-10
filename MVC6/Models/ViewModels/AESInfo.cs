using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.ViewModels
{
	//Test
	public class AESInfo
	{
		[Required(ErrorMessage = "Please input your ID")]
		[MinLength(6, ErrorMessage = "Minmum 6")]
		[Display(Name = "UserID")]
		public string UserID { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Please input your Password")]
		[MinLength(6, ErrorMessage = "Minmum 6")]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.MultilineText)]
		[Display(Name = "EncInfo")]
		public string EncUserInfo { get; set; }

		[Display(Name = "DncInfo")]
		public string DecUserInfo { get; set; }
	}
}

