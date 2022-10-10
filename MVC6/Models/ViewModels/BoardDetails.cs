using System;
using System.ComponentModel.DataAnnotations;
using MVC6.Models.DataModels;

namespace MVC6.Models.ViewModels
{
	public class BoardDetails
	{
		[Required(ErrorMessage = "No Error")]
		public int No { get; set; }

		[Required(ErrorMessage = "Please Input Title")]
		[Display(Name = "Title")]
		public string Title { get; set; }

		[Required(ErrorMessage = "Please Input Content")]
		[Display(Name = "Content")]
		public string Content { get; set; }

		[Display(Name = "UserID")]
		[Required(ErrorMessage = "UserID Error")]
		public string UserID { get; set; }

		[Display(Name = "Name")]
		[Required(ErrorMessage = "Name Error")]
		public string UserName { get; set; }

		public DateTime UpdatedAt { get; set; }

		public IEnumerable<BoardReply> replyList { get; set; }
	}
}

