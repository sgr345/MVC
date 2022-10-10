using Microsoft.AspNetCore.Mvc;
using MVC6.Controllers.Interfaces;
using MVC6.Extensions;
using MVC6.Models.ViewModels;
using System.Net.Mail;

namespace MVC6.Controllers
{
    public class CartController : Controller
    {
        private HttpContext _context;
        private string _sessionKeyCartName = "_sessionCartKey";
        private ILogger<CartController> _logger;
        private ICart _cart;

        public CartController(IHttpContextAccessor accessor, ICart cart, ILogger<CartController> logger)
        {
            _context = accessor.HttpContext;
            _cart = cart;
            _logger = logger;
        }

        #region private
        private List<ItemInfo> GetCartInfo(ref string message)
        {
            var cartInfos = _context.Session.Get<List<ItemInfo>>(key: _sessionKeyCartName);
            if (cartInfos == null || cartInfos.Count == 1 || cartInfos.Count == 0)
            {
                message = "Nothing in Cart";
            }
            return cartInfos;
        }

        private void SetCartInfos(ItemInfo item, List<ItemInfo> cartInfos = null)
        {
            if (cartInfos == null)
            {
                cartInfos = _context.Session.Get<List<ItemInfo>>(_sessionKeyCartName);

                if (cartInfos == null)
                {
                    cartInfos = new List<ItemInfo>();
                }
            }
            cartInfos.Add(item);

            _context.Session.Set<List<ItemInfo>>(_sessionKeyCartName, cartInfos);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCart()
        {
            //MailMessage message = new MailMessage();
            //message.From = new MailAddress("sgr333@naver.com");
            //message.To.Add(new MailAddress("sgr345@gmail.com"));
            //message.IsBodyHtml = true;

            //message.Subject = "TestMail";
            //message.Body = "BodyTest";
            //message.SubjectEncoding = System.Text.Encoding.UTF8;
            //message.BodyEncoding = System.Text.Encoding.UTF8;

            //SmtpClient client = new SmtpClient();
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.EnableSsl = true;
            //client.Host = "smtp.naver.com";
            //client.Port = 587;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new System.Net.NetworkCredential("sgr333", "G7XQTBDVL13C");

            //client.Send(message);
            if (_context.User.Identity.IsAuthenticated)
            {
               var test = _context.User.IsInRole("SystemUser");
            }

            Random radom = new Random();
             SetCartInfos(new ItemInfo() { ItemNo = radom.Next(100).ToString(), ItemName = Guid.NewGuid().ToString() });
            if(_context.User.Identity.Name != null)
            {
                var list = _context.Session.Get<List<ItemInfo>>(_sessionKeyCartName);
                _cart.SaveCartInfo(list ,_context.User.Identity.Name);
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveCart()
        {
            string message = string.Empty;
            var cartInfos = GetCartInfo(ref message);
            if (cartInfos != null && cartInfos.Count > 0)
            {
                _context.Session.Remove(key: _sessionKeyCartName);
            }
            if (_context.User.Identity.Name != null)
            {
                var list = _context.Session.Get<List<ItemInfo>>(_sessionKeyCartName);
                _cart.SaveCartInfo(list, _context.User.Identity.Name);
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveItem(string itemName)
        {
            string message = string.Empty;
            var cartInfos = GetCartInfo(ref message);
            if(cartInfos != null)
            {
                cartInfos.Remove(cartInfos.Where(item => item.ItemName.Equals(itemName)).FirstOrDefault());
            }
            _context.Session.Set<List<ItemInfo>>(_sessionKeyCartName, cartInfos);

            if (_context.User.Identity.Name != null)
            {
                var list = _context.Session.Get<List<ItemInfo>>(_sessionKeyCartName);
                _cart.SaveCartInfo(list, _context.User.Identity.Name);
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public IActionResult Index()
        {
            string message = string.Empty;
            var cartInfos = GetCartInfo(ref message);
            
            if (_context.User.Identity.Name != null)
            {
                cartInfos =_cart.GetCartInfoByUserID(_context.User.Identity.Name);
                _context.Session.Set<List<ItemInfo>>(_sessionKeyCartName, cartInfos);
            }
            ViewData["Message"] = message;

            return View(cartInfos);
        }
    }
}