using System;
namespace MVC6.Models.DataModels
{
	public class BoardReply
	{
		public int Board_No { get; set; }
		public int No { get; set; }
		public string Contents { get; set; }
		public string UserID { get; set; }
		public string UserName { get; set; }

		public DateTime UpdatedAt { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}

