using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace PROJEKTDB.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // LOGIN ADMIN (i thjeshtë)
            if (username == "admin" && password == "123")
            {
                // ruaj login në session
                HttpContext.Session.SetString("IsAdmin", "true");

                // 👉 RIDREJTO NE HOME
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Username ose password gabim";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
