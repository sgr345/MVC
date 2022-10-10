using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MVC6.Controllers.Bridges;
using MVC6.Controllers.Interfaces;
using MVC6.Models;
using MVC6.Models.DataModels;

namespace MVC6.Controllers.Services
{
	public class PasswordHasher : IPasswordHasher
	{
        #region private
        private string GetGUIDSalt()
        {
            return Guid.NewGuid().ToString();
        }

        private string GetRngSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private string GetPasswordHasher(string userId, string password, string guiSalt, string rngSalt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: userId.ToLower() + password + guiSalt,
            salt: Encoding.UTF8.GetBytes(rngSalt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 45000,
            numBytesRequested: 256 / 8));
        }

        private bool CheckThePasswordInfo(string userId, string password, string guiSalt, string rngSalt, string passwordHash)
        {
            return GetPasswordHasher(userId, password, guiSalt, rngSalt).Equals(passwordHash);
        }

        private PasswordHashInfo PasswordInfo(string userId, string password)
        {
            string guiSalt = GetGUIDSalt();
            string rngSalt = GetRngSalt();
            var passwordInfo = new PasswordHashInfo() {
                GUIDSalt = guiSalt,
                RNGSalt = rngSalt,
                PasswordHash = GetPasswordHasher(userId, password, guiSalt, rngSalt)
            };
            return passwordInfo;
        }
        #endregion

        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRngSalt()
        {
            return GetRngSalt();
        }

        string IPasswordHasher.GetPasswordHasher(string userId, string password, string guiSalt, string rngSalt)
        {
            return GetPasswordHasher(userId,password,guiSalt,rngSalt);
        }

        bool IPasswordHasher.CheckThePasswordInfo(string userId, string password, string guiSalt, string rngSalt, string passwordHash)
        {
            return CheckThePasswordInfo(userId, password, guiSalt, rngSalt, passwordHash);
        }
        /// <summary>
        /// For Register
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns>PasswordHashInfo Instance</returns>
        public PasswordHashInfo GetPasswordInfo(string userId, string password)
        {
            return PasswordInfo(userId, password);
        }
    }
}

