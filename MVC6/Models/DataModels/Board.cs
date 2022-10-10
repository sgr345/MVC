using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.DataModels
{
	public class Board
	{
		public int No { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string UserID { get; set; }

		public string UserName { get; set; }

		public int ReadCount { get; set; }

		public DateTime UpdatedAt { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}

