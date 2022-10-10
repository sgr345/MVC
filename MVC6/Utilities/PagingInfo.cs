using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Utilities
{
	public class PagingInfo
	{
		public int TotalEntries { get; set; }

		public int TotalItems { get; set; }

		public int NumberLinksPerPage { get; set; }

		[Required(ErrorMessage = "Please Input CurrentPage")]
		[Range(1, int.MaxValue, ErrorMessage = "CurrentPage must be bigger then {1}")]
		public int CurrentPage { get; set; }

		public int TotalPage
			=> ((int)Math.Ceiling((decimal)TotalItems / ItemsPerPage));

		public int StartPage { get; set; }

		public int EndPage { get; set; }

		[Required(ErrorMessage = "Please Input ItemsPerPage")]
		[Range(1, int.MaxValue, ErrorMessage = "ItemPerPage must be bigger then {1}")]
		public int ItemsPerPage { get; set; }

		public int FirstItem { get; set; }

		public int LastItem { get; set; }

		public string SearchSubject { get; set; }

		public string SearchKeyword { get; set; }
	}
}

