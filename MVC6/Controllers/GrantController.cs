using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MVC6.Controllers.Interfaces;
using System.Linq;
using MVC6.Models.ViewModels;
using MVC6.Utilities;

namespace MVC6.Controllers
{
    [Authorize(Roles = "SuperUser, SystemUser")]
    public class GrantController : Controller
    {
        private ILogger<GrantController> logger;
        private IGrant grant;
        private HttpContext _context;
        private ConfigurationManager config = WebApplication.CreateBuilder().Configuration;

        public GrantController(IHttpContextAccessor accessor, IGrant grant, ILogger<GrantController> logger)
        {
            this.logger = logger;
            this.grant = grant;
            _context = accessor.HttpContext;
        }

        [HttpGet("/{controller}/Index")]
        public async Task<IActionResult> Index()
        {

            var user = await grant.GetUserListAsync();

            var grantInfo = new GrantInfo()
            {
                Users = user.ToList()
            };
            return View(grantInfo);
        }
        [HttpGet]
        public IActionResult Ajax()
        {
            return View();
        }
        [HttpGet("/{controller}/List/{PageNo?}/{PageSize?}/{SearchName?}")]
        public async Task<IActionResult> ListAsync(int pageNo = 1, int? pageSize = null, string searchName = null)
        {
            int itemsPerPage = (int)(pageSize != null && pageSize > 0 ?
                                Convert.ToInt32(pageSize) : Convert.ToInt32(config["Paging:ItemsPerPage"]));
            int numberLinksPerPage = Convert.ToInt32(config["Paging:NumberLinksPerPage"]);

            var user = await grant.GetGrantDataInfoAsync(pageNo, itemsPerPage, numberLinksPerPage, searchName);

            return View("List", user);
        }
        [HttpPost("/{controller}/List/{PageNo?}/{PageSize}/{SearchName?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListAsync(PagingInfo pagingInfo)
        {
            string message = string.Empty;
            if (!ModelState.IsValid)
            {
                if(pagingInfo.CurrentPage < 1)
                {
                    ModelState.AddModelError(string.Empty, "CurrentPageError");
                }
                if(pagingInfo.ItemsPerPage < 1)
                {
                    ModelState.AddModelError(string.Empty, "ItemPerPageError");
                }

                var user = await grant.GetGrantDataInfoAsync(pagingInfo.CurrentPage,
                                                            pagingInfo.ItemsPerPage,
                                                        Convert.ToInt32(config.GetSection("Paging").GetValue<string>("NumberLinksPerPage")),
                                                        pagingInfo.SearchKeyword);
                return View("List",user);
            }
            return RedirectToAction("List", "Grant", new {PageNo = pagingInfo.CurrentPage,
                                                          PageSize = pagingInfo.ItemsPerPage,
                                                          SearchName = pagingInfo.SearchKeyword});
        }
    }
}