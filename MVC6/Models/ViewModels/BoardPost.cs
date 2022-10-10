using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.ViewModels
{
	public class BoardPost
	{
		[Display(Name = "Title")]
		[Required(ErrorMessage = "Please Input Title")]
		public string Title { get; set; }

		[Display(Name = "Content")]
		[Required(ErrorMessage = "Please Input Content")]
		public string Content { get; set; }

		[Display(Name = "UserID")]
		[Required(ErrorMessage = "UserID Error")]
		public string UserID { get; set; }

		[Display(Name = "Name")]
		[Required(ErrorMessage = "Name Error")]
		public string UserName { get; set; }
	}
}

