using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MVC6.Controllers.Interfaces;
using MVC6.Extensions;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers
{
    public class DataController : Controller
    {
        private IDataProtector _protector;
        private HttpContext _context;

        public DataController(IHttpContextAccessor accessor,IDataProtectionProvider provider, ICart cart)
        {
            _context = accessor.HttpContext;
            _protector = provider.CreateProtector("MVC6.Data.v1");
        }

        #region AES
        [HttpGet]
        [Authorize(Roles = "GeneralUser,SuperUser,SystemUser")]
        public IActionResult AES()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GeneralUser,SuperUser,SystemUser")]
        public IActionResult AES(AESInfo aes)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                string userInfo = aes.UserID + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo);
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo);
                ViewData["Message"] = "Successed Encrypt and Decrypt";
                return View(aes);
            }
            else
            {
                message = "AESInfo is not corrected";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(aes);
        }
        #endregion

        
    }
}