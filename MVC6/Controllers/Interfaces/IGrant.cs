using System;
using MVC6.Models.DataModels;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Interfaces
{
	public interface IGrant
	{
		Task<IEnumerable<User>> GetUserListAsync();

		Task<GrantDataInfo> GetGrantDataInfoAsync(int pageNo, int itemPerPage, int numberLInksPerPage, string searchKeyword);
	}
}

