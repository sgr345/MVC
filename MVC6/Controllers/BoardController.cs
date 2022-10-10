using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC6.Controllers.Interfaces;
using MVC6.Models.ViewModels;
using MVC6.Utilities;

namespace MVC6.Controllers
{
    public class BoardController : Controller
    {
        private ILogger<BoardController> logger;
        private IBoard board;
        private HttpContext _context;
        private ConfigurationManager config = WebApplication.CreateBuilder().Configuration;

        public BoardController(IHttpContextAccessor accessor, IBoard board, ILogger<BoardController> logger)
        {
            this.logger = logger;
            this.board = board;
            _context = accessor.HttpContext;
        }

        [HttpGet("/{controller}/List/{PageNo?}/{PageSize?}/{SearchName?}")]
        public async Task<IActionResult> Index(string searchSubject, string keyWord, int pageNo = 1, int? pageSize = null)
        {
            var comboList = new Dictionary<string,string>{
                { "Title","Title" },
                { "Content" ,"Content"},
                { "UserName", "UserName"},
                { "Title + Content","Title,Content" }
            };
            ViewBag.comboList = new SelectList(comboList, "Value","Key", searchSubject);

            ViewBag.CurrentPage = pageNo;
            int itemsPerPage = (int)(pageSize != null && pageSize > 0 ?
                               Convert.ToInt32(pageSize) : Convert.ToInt32(config["Paging:ItemsPerPage"]));
            int numberLinksPerPage = Convert.ToInt32(config["Paging:NumberLinksPerPage"]);
            var boardInfo = await board.GetBoardList(pageNo, itemsPerPage,numberLinksPerPage, searchSubject, keyWord);
            
            return View(boardInfo);
        }

        [HttpGet]
        [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
        public IActionResult Post()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
        public IActionResult Post(BoardPost boardPost)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if(board.PostSubmit(boardPost) > 0)
                {
                    return RedirectToAction("Index", "Board");
                }
                else
                {
                    message = "Insert Failed";
                }
            }
            else
            {
                message = "BoardInfo is not correct";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(boardPost);
        }
        [HttpGet]
        public IActionResult Details(int no, int pageNo, string searchSubject, string keyWord)
        {
            ViewBag.CurrentPage = pageNo;
            ViewBag.SearchSubject = searchSubject;
            ViewBag.KeyWord = keyWord;
            string message = string.Empty;

            board.UpdateReadCount(no);
            var result = board.GetBoardInfo(no);
            if(result == null)
            {
                message = "BoardGetErrors";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(result);
        }

        [HttpGet]
        public IActionResult Modify(int no, int pageNo, string searchSubject, string keyWord)
        {
            ViewBag.CurrentPage = pageNo;
            ViewBag.SearchSubject = searchSubject;
            ViewBag.KeyWord = keyWord;
            string message = string.Empty;
            var result = board.GetBoardInfo(no);
            if (result == null)
            {
                message = "Error";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Modify(BoardDetails boardModify, int pageNo, string searchSubject, string keyWord)
        {
            ViewBag.CurrentPage = pageNo;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if(board.UpdateBoardInfo(boardModify) > 0)
                {
                    return RedirectToAction("Details", "Board",new { no = boardModify.No, pageNo = pageNo, searchSubject = searchSubject, keyWord = keyWord });
                }
                else
                {
                    message = "Failed to Update";
                }
            }
            else
            {
                message = "BoardInfo is not corrected";
            }
            ModelState.AddModelError(string.Empty,message);
            return View(boardModify);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int no, int pageNo, string searchSubject, string keyWord)
        {
            string message = string.Empty;
            if(board.DeleteBoardInfo(no) == 0)
            {
                message = "Failed to elete";
            }
            ModelState.AddModelError(string.Empty, message);
            return RedirectToAction("Index","Board", new { pageNo = pageNo, searchSubject = searchSubject, keyWord = keyWord });
        }
    }
}