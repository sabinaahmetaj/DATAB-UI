using Microsoft.AspNetCore.Mvc;

namespace PROJEKTDB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
