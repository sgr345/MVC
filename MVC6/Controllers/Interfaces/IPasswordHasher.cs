using System;
using MVC6.Controllers.Bridges;

namespace MVC6.Controllers.Interfaces
{
	public interface IPasswordHasher
	{
		string GetGUIDSalt();
		string GetRngSalt();
		string GetPasswordHasher(string userId, string password, string guiSalt, string rngSalt);
		bool CheckThePasswordInfo(string userId, string password, string guiSalt, string rngSalt, string passwordHash);
		PasswordHashInfo GetPasswordInfo(string userId, string password);
	}
}

