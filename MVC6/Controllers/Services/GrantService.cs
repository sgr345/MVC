using System;
using System.Text;
using MVC6.Controllers.Interfaces;
using MVC6.Models;
using MVC6.Models.DataModels;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Services
{
    public class GrantService : IGrant
    {

        private ILogger<GrantService> _logger;
        private IDBConnection conn;
        public GrantService(IDBConnection conn, ILogger<GrantService> logger)
        {
            this.conn = conn;
            _logger = logger;
        }

        #region Private
        private async Task<IEnumerable<User>> GetUserListAsync()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT");
                sql.AppendLine("*");
                sql.AppendLine("FROM");
                sql.AppendLine("TBL_USER U");
                return await conn.QueryAsync<User>(sql.ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<GrantDataInfo> GetGrantDataInfoAsync(int pageNo, int itemPerPage, int numberLInksPerPage, string searchKeyword)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("*");
            sql.AppendLine("FROM");
            sql.AppendLine("TBL_USER U");
            sql.AppendLine("WHERE 0=0");
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                sql.AppendLine($"AND UserID like '%{searchKeyword}%' OR UserName like '%{searchKeyword}%'");
            }
            sql.AppendLine("ORDER BY UserID");
            sql.AppendLine($"LIMIT {itemPerPage} OFFSET {(pageNo - 1) * itemPerPage}");

            IEnumerable<User> user = await conn.QueryAsync<User>(sql.ToString());
            int totalGrantCount = await conn.QueryFirstAsync<int>("SELECT COUNT(*) FROM TBL_USER");

            var grantDataInfo = new GrantDataInfo() {
                Users = user.ToList(),
                PagingInfo = new Utilities.PagingInfo
                {
                    TotalEntries = totalGrantCount,
                    TotalItems = totalGrantCount,
                    NumberLinksPerPage = numberLInksPerPage,
                    CurrentPage = pageNo,
                    ItemsPerPage = itemPerPage,
                    FirstItem = itemPerPage * (pageNo - 1) + 1,
                    LastItem = itemPerPage * pageNo,
                    SearchKeyword = searchKeyword
                }
            };
            return grantDataInfo;
        }
        #endregion

        Task<IEnumerable<User>> IGrant.GetUserListAsync()
        {
            return GetUserListAsync();
        }

        Task<GrantDataInfo> IGrant.GetGrantDataInfoAsync(int pageNo, int itemPerPage, int numberLInksPerPage, string searchKeyword)
        {
            return GetGrantDataInfoAsync(pageNo, itemPerPage, numberLInksPerPage, searchKeyword);
        }
    }
}

