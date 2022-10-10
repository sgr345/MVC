using System.Text;
using System.Transactions;
using MVC6.Controllers.Interfaces;
using MVC6.Models;
using MVC6.Models.DataModels;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Services
{
    public class UserService : IUser
    {
        private IDBConnection conn;
        private IPasswordHasher hasher;
        private ILogger<UserService> logger;
        public UserService(IDBConnection conn, IPasswordHasher hasher, ILogger<UserService> logger)
        {
            this.conn = conn;
            this.hasher = hasher;
            this.logger = logger;
        }

        #region private
        private User GetUserInfoByID(string userId)
        {
            try
            {
                User user = null;
                StringBuilder sql = new StringBuilder();
                using (var trans = new TransactionScope())
                {
                    sql.AppendLine("SELECT");
                    sql.AppendLine("*");
                    sql.AppendLine("FROM");
                    sql.AppendLine("TBL_User");
                    sql.AppendLine("WHERE");
                    sql.AppendLine("UserID = '" + userId + "'");
                    user = conn.Query<User>(sql.ToString()).FirstOrDefault();
                    trans.Complete();
                }
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }
        private UserRolesByUser GetUserRolesByUserInfo(string userId)
        {
            try
            {
                UserRolesByUser userRolesByUsers = null;
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT");
                sql.AppendLine("*");
                sql.AppendLine("FROM");
                sql.AppendLine("TBL_UserRolesByUser");
                sql.AppendLine("WHERE");
                sql.AppendLine("UserID = '" + userId + "'");
                userRolesByUsers = conn.Query<UserRolesByUser>(sql.ToString()).FirstOrDefault();

                userRolesByUsers.UserRole = GetUserRole(userRolesByUsers.RoleID);
                return userRolesByUsers;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }
        private UserRole GetUserRole(string roleId)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT");
                sql.AppendLine("*");
                sql.AppendLine("FROM");
                sql.AppendLine("TBL_UserRole");
                sql.AppendLine("WHERE");
                sql.AppendLine("RoleID = '" + roleId + "'");
                return conn.Query<UserRole>(sql.ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        private int RegisterUser(RegisterInfo register)
        {
            try
            {
                int result = 0;
                var utcNow = DateTime.UtcNow;
                using (var trans = new TransactionScope())
                {
                    StringBuilder userSql = new StringBuilder();
                    userSql.AppendLine("INSERT INTO TBL_User");
                    userSql.AppendLine("(");
                    userSql.AppendLine("UserID,");
                    userSql.AppendLine("UserName,");
                    userSql.AppendLine("UserEmail,");
                    userSql.AppendLine("GUIDSalt,");
                    userSql.AppendLine("RNGSalt,");
                    userSql.AppendLine("PasswordHash,");
                    userSql.AppendLine("AccessFailedCount,");
                    userSql.AppendLine("IsMembershipWithdrawn,");
                    userSql.AppendLine("JoinedUtcDate");
                    userSql.AppendLine(")");
                    userSql.AppendLine("VALUES");
                    userSql.AppendLine("(");
                    userSql.AppendLine("@UserID,");
                    userSql.AppendLine("@UserName,");
                    userSql.AppendLine("@UserEmail,");
                    userSql.AppendLine("@GUIDSalt,");
                    userSql.AppendLine("@RNGSalt,");
                    userSql.AppendLine("@PasswordHash,");
                    userSql.AppendLine("@AccessFailedCount,");
                    userSql.AppendLine("@IsMembershipWithdrawn,");
                    userSql.AppendLine("@JoinedUtcDate");
                    userSql.AppendLine(");");
                    var passwordInfo = hasher.GetPasswordInfo(register.UserID.ToLower(), register.Password);
                    var user = new User()
                    {
                        UserID = register.UserID.ToLower(),
                        UserName = register.UserName,
                        UserEmail = register.UserEmail,
                        GUIDSalt = passwordInfo.GUIDSalt,
                        RNGSalt = passwordInfo.RNGSalt,
                        PasswordHash = passwordInfo.PasswordHash,
                        AccessFailedCount = 0,
                        IsMembershipWithdrawn = false,
                        JoinedUtcDate = utcNow
                    };
                    result += conn.Execute(userSql.ToString(), user);
                    StringBuilder userRole = new StringBuilder();
                    userRole.AppendLine("INSERT INTO TBL_UserRolesByUser ");
                    userRole.AppendLine("( ");
                    userRole.AppendLine("UserID, ");
                    userRole.AppendLine("RoleID, ");
                    userRole.AppendLine("OwnedUtcDate ");
                    userRole.AppendLine(") ");
                    userRole.AppendLine("VALUES ");
                    userRole.AppendLine("( ");
                    userRole.AppendLine("@UserID, ");
                    userRole.AppendLine("@RoleID, ");
                    userRole.AppendLine("@OwnedUtcDate ");
                    userRole.AppendLine("); ");
                    var userRolesByUser = new UserRolesByUser()
                    {
                        UserID = register.UserID.ToLower(),
                        RoleID = "AssociateUser",
                        OwnedUtcDate = utcNow
                    };
                    result += conn.Execute(userRole.ToString(), userRolesByUser);
                    trans.Complete();
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return 0;
            }

        }
        private UserInfo GetUserInfoForUpdate(string userId)
        {
            try
            {
                var user = GetUserInfoByID(userId);
                var userInfo = new UserInfo()
                {
                    UserID = user.UserID,
                    UserName = user.UserName,
                    UserEmail = user.UserEmail,
                    ChangeInfo = new ChangeInfo()
                    {
                        UserName = user.UserName,
                        UserEmail = user.UserEmail
                    }
                };
                return userInfo;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        private int UpdateUser(UserInfo user)
        {
            try
            {
                int result = 0;
                User userInfo = GetUserInfoByID(user.UserID);
                if (userInfo == null)
                {
                    return 0;
                }
                bool check = hasher.CheckThePasswordInfo(user.UserID, user.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);
                if (check)
                {
                    StringBuilder updSql = new StringBuilder();
                    updSql.AppendLine("UPDATE TBL_User SET ");
                    updSql.AppendLine("UserName = @UserName,");
                    updSql.AppendLine("UserEmail = @UserEmail");
                    updSql.AppendLine("WHERE");
                    updSql.AppendLine("UserID = @UserID ;");
                    result = conn.Execute(updSql.ToString(), user);
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return 0;
            }

        }

        private bool CompareInfo(UserInfo user)
        {
            return user.ChangeInfo.Equals(user);
        }

        private int WithdrawnUser(UserInfo user)
        {
            try
            {
                int result = 0;
                User userInfo = GetUserInfoByID(user.UserID);
                if (user == null)
                {
                    return 0;
                }
                bool check = hasher.CheckThePasswordInfo(user.UserID, user.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);

                if (check)
                {
                    using (var trans = new TransactionScope())
                    {
                        StringBuilder delUser = new StringBuilder();
                        delUser.AppendLine("DELETE FROM TBL_User");
                        delUser.AppendLine("WHERE");
                        delUser.AppendLine("UserID = @UserID");
                        result += conn.Execute(delUser.ToString(), userInfo);

                        StringBuilder delRoles = new StringBuilder();
                        delRoles.AppendLine("DELETE FROM TBL_UserRolesByUser");
                        delRoles.AppendLine("WHERE");
                        delRoles.AppendLine("UserID = @UserID");
                        result += conn.Execute(delRoles.ToString(), userInfo);
                        trans.Complete();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return 0;
            }
        }
        #endregion

        public bool MatchUserInfo(LoginInfo login)
        {
            User userInfo = GetUserInfoByID(login.UserID);
            if (userInfo == null)
            {
                return false;
            }
            return hasher.CheckThePasswordInfo(login.UserID, login.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);
        }

        User IUser.GetUserInfoByID(string userId)
        {
            return GetUserInfoByID(userId);
        }

        public UserRolesByUser GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfo(userId);
        }

        int IUser.RegisterUser(RegisterInfo register)
        {
            return RegisterUser(register);
        }

        UserInfo IUser.GetUserInfoForUpdate(string userId)
        {
            return GetUserInfoForUpdate(userId);
        }

        int IUser.UpdateUser(UserInfo user)
        {
            return UpdateUser(user);
        }

        bool IUser.CompareInfo(UserInfo user)
        {
            return CompareInfo(user);
        }

        int IUser.WithdrawnUser(UserInfo user)
        {
            return WithdrawnUser(user);
        }
    }
}

