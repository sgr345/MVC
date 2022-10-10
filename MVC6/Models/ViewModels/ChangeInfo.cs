using System;
using System.ComponentModel.DataAnnotations;

namespace MVC6.Models.ViewModels
{
	public class ChangeInfo
	{
		public string UserName { get; set; }

		[DataType(DataType.EmailAddress)]
		public string UserEmail { get; set; }

		public bool Equals(UserInfo other)
        {
			if(!string.Equals(UserName, other.UserName))
            {
				return false;
            }
			if (!string.Equals(UserEmail, other.UserEmail))
			{
				return false;
			}
			return true;
        }
	}
}

