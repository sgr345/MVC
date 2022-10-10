using System;
using MVC6.Models.DataModels;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Interfaces
{
	public interface IUser
	{
		bool MatchUserInfo(LoginInfo login);
		User GetUserInfoByID(string userId);
		UserRolesByUser GetRolesOwnedByUser(string userId);
		int RegisterUser(RegisterInfo register);
		UserInfo GetUserInfoForUpdate(string userId);
		int UpdateUser(UserInfo user);
		bool CompareInfo(UserInfo user);
		int WithdrawnUser(UserInfo user);
	}
}

