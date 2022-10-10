using System;
using System.Text;
using System.Transactions;
using MVC6.Controllers.Interfaces;
using MVC6.Models;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Services
{
    public class CartService : ICart
    {
        private IDBConnection conn;
        private ILogger<CartService> logger;
        public CartService(IDBConnection conn, ILogger<CartService> logger)
        {
            this.conn = conn;
            this.logger = logger;
        }

        #region private
        private List<ItemInfo> GetCartInfoByUserID(string userId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT ");
                sql.AppendLine("ItemNo,");
                sql.AppendLine("ItemName");
                sql.AppendLine("FROM");
                sql.AppendLine("TBL_ItemList");
                sql.AppendLine("WHERE");
                sql.AppendLine($"UserID = '{userId}'");
                return conn.Query<ItemInfo>(sql.ToString()).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }
        private void Regist(List<ItemInfo> items, string userId)
        {
            try
            {
                foreach (var item in items)
                {
                    using (var trans = new TransactionScope())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("INSERT INTO TBL_ItemList");
                        sql.AppendLine("(");
                        sql.AppendLine("UserID,");
                        sql.AppendLine("ItemNo,");
                        sql.AppendLine("ItemName");
                        sql.AppendLine(")");
                        sql.AppendLine("VALUES");
                        sql.AppendLine("(");
                        sql.AppendLine($"'{userId}',");
                        sql.AppendLine($"'{item.ItemNo}',");
                        sql.AppendLine($"'{item.ItemName}'");
                        sql.AppendLine(")");
                        conn.Execute(sql.ToString());
                        trans.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        private void Delete(List<ItemInfo> items, string userId)
        {
            try
            {
                foreach (var item in items)
                {
                    using (var trans = new TransactionScope())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("DELETE FROM TBL_ItemList");
                        sql.AppendLine("WHERE");
                        sql.AppendLine($"UserID = '{userId}' AND");
                        sql.AppendLine($"ItemNo = '{item.ItemNo}' AND");
                        sql.AppendLine($"ItemName = '{item.ItemName}'");
                        conn.Execute(sql.ToString());
                        trans.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }
        private void DeleteAll(string userId)
        {
            try
            {
                using (var trans = new TransactionScope())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("DELETE FROM TBL_ItemList");
                    sql.AppendLine("WHERE");
                    sql.AppendLine($"UserID = '{userId}'");
                    conn.Execute(sql.ToString());
                    trans.Complete();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
        }

        private bool SaveCartInfo(List<ItemInfo> iteminfos, string userId)
        {
            try
            {
                if(iteminfos == null)
                {
                    DeleteAll(userId);
                }
                var cartData = GetCartInfoByUserID(userId);
                if (cartData != null)
                {
                    if (cartData.Count() != 0)
                    {
                        var registList = iteminfos.Where(i => !cartData.Any(j => j.ItemNo == i.ItemNo)).ToList();
                        if (registList.Count != 0)
                        {
                            Regist(registList, userId);
                        }
                        var deleteList = cartData.Where(i => !iteminfos.Any(j => j.ItemNo == i.ItemNo)).ToList();
                        if (deleteList.Count != 0)
                        {
                            Delete(deleteList, userId);
                        }
                    }
                    else
                    {
                        Regist(iteminfos, userId);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return false;
            }
        }
        #endregion

        bool ICart.SaveCartInfo(List<ItemInfo> iteminfos, string userId)
        {
            return SaveCartInfo(iteminfos, userId);
        }

        List<ItemInfo> ICart.GetCartInfoByUserID(string userId)
        {
            return GetCartInfoByUserID(userId);
        }
    }
}

