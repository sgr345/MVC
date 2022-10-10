using System;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Interfaces
{
	public interface IBoard
	{
		public int PostSubmit(BoardPost board);
		public Task<BoardInfo> GetBoardList(int pageNo, int itemPerPage, int numberLInksPerPage, string cmb, string keyWord);
		public BoardDetails GetBoardInfo(int no);
		public int DeleteBoardInfo(int no);
		public int UpdateBoardInfo(BoardDetails board);
		public int UpdateReadCount(int no);
	}
}

