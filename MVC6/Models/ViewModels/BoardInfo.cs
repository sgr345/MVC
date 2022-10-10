using System;
using System.ComponentModel.DataAnnotations;
using MVC6.Models.DataModels;
using MVC6.Utilities;

namespace MVC6.Models.ViewModels
{
	public class BoardInfo
	{
		public List<Board> boardList { get; set; }

		public PagingInfo PagingInfo { get; set; }
	}
}

