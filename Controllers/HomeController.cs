using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVCDemo1.Models;
using System.Diagnostics;

namespace MVCDemo1.Controllers
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("UserID") == null)
            {
                filterContext.Result = new RedirectResult("~/User/Login");
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "-1";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(context);
        }
    }

    [CheckAccess]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }

        //create cookie
        public IActionResult CreateCookie()
        {
            string key = "UserName";
            string value = "Parmar Pinal";
            CookieOptions op = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(5),
            };
            Response.Cookies.Append(key, value, op);
            return View("Index");
        }

        //read cookie
        public IActionResult ReadCookie()
        {
            string key = "UserName";
            var ans = Request.Cookies[key]; 
            return View("Index");
        }

        //Remove cookie
        public IActionResult RemoveCookie()
        {
            string key = "UserName";
            string value = "Parmar Pinal";
            CookieOptions op = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1),
            };
            Response.Cookies.Append(key, value, op);
            return View("Index");
        }

        [Route("Home/Detail/{n?}")]
        public string getDetails(String n)
        {
            return "Name details = "+n;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
