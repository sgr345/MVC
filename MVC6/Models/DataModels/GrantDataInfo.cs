using System;
using MVC6.Utilities;

namespace MVC6.Models.DataModels
{
    public class GrantDataInfo
    {
        public List<User> Users = new List<User>();

        public PagingInfo PagingInfo { get; set; }
    }
}

