using System;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Interfaces
{
	public interface ICart
	{
		public bool SaveCartInfo(List<ItemInfo> iteminfos, string userId);
		public List<ItemInfo> GetCartInfoByUserID(string userId);
	}
}

