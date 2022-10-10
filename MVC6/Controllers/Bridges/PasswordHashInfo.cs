using System;
namespace MVC6.Controllers.Bridges
{
	public class PasswordHashInfo
	{
		public string GUIDSalt { get; set; }
		public string RNGSalt { get; set; }
		public string PasswordHash { get; set; }
	}
}

