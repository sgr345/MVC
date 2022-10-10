using Microsoft.AspNetCore.Mvc;
using MVC6.Controllers.Interfaces;
using MVC6.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using MVC6.Extensions;
using Microsoft.AspNetCore.Identity;

namespace MVC6.Controllers
{
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
    public class MembershipController : Controller
    {
        private IUser _user;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IUser user)
        {
            _context = accessor.HttpContext;
            _user = user;
        }
        #region private
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginInfo loginInfo, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (_user.MatchUserInfo(loginInfo))
                {
                    var userInfo = _user.GetUserInfoByID(loginInfo.UserID);
                    var roles = _user.GetRolesOwnedByUser(loginInfo.UserID);
                    var userTopRole = roles;
                    
                    var identity = new ClaimsIdentity(claims: new[] {
                        new Claim(type:ClaimTypes.Name,
                                    value:userInfo.UserID),
                        new Claim(type:ClaimTypes.Role,
                                    value:userTopRole.RoleID),
                        new Claim(type:ClaimTypes.UserData,
                                    value:userInfo.UserName)
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                                principal: new ClaimsPrincipal(identity: identity),
                                                properties: new AuthenticationProperties()
                                                {
                                                    IsPersistent = loginInfo.RememberMe,
                                                    ExpiresUtc = loginInfo.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                                });
                    
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "Failed to Login";
                }
            }
            else
            {
                message = "LoginInfo is not correct";
            }
            ModelState.AddModelError(string.Empty, message);
            return View("Login", loginInfo);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Register(RegisterInfo register, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;
            if (ModelState.IsValid)
            {

                if (_user.RegisterUser(register) > 0)
                {
                    TempData["Message"] = "Successed to regist";
                    return RedirectToAction("Login", "Membership");
                }
                else
                {
                    message = "Failed to register";
                }
            }
            else
            {
                message = "RegisterInfo is not corrected";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(register);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            UserInfo user = _user.GetUserInfoForUpdate(_context.User.Identity.Name);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserInfo user)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (_user.CompareInfo(user))
                {
                    message = "No Changed";
                }
                else
                {
                    if (_user.UpdateUser(user) > 0)
                    {
                        
                        //Update Principal
                        var orgUserData = _context.User.GetClaimValue(ClaimTypes.UserData).Split("|");
                        _context.User.AddUpdateClaim(ClaimTypes.UserData,user.UserName);
                        await _context.RefreshSignInAsync(_context.User);

                        TempData["Message"] = "Successed to modify";
                        return RedirectToAction("Profile", "Membership");
                    }
                    else
                    {
                        message = "Failed to modify";
                    }
                }
            }
            else
            {
                message = "UserInfo is not corrected";
            }
            ViewData["Message"] = message;
            ModelState.AddModelError(string.Empty, message);
            return View(user);
        }

        [HttpPost("/Withdrawn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawnAsync(UserInfo userInfo)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (_user.WithdrawnUser(userInfo) > 0)
                {
                    TempData["Message"] = "Successed to withdrawn";
                    await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    message = "Failed to withdrawn";
                }
            }
            else
            {
                message = "UserInfo is not corrected";
            }
            ViewData["Message"] = message;
            return View("Profile", userInfo);
        }

        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            _context.Session.Clear();
            TempData["Message"] = "Success Logout";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out StringValues paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            ViewData["Message"] = $"{paramReturnUrl} <br />" +
                                    "Please call to Manager<br />";
            return View();
        }
    }
}