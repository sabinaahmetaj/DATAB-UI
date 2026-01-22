using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROJEKTDB.Data;
using System.Linq;

namespace PROJEKTDB.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Login()
        {
            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // 1) ADMIN statik
            if (username == "admin" && password == "123")
            {
                HttpContext.Session.SetString("PerRole", "ADMIN");
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index", "Home");
            }

            // 2) ARTIST / MANAGER: username = PER_EM, password = 123
            if (password == "123")
            {
                var user = _db.Persons.FirstOrDefault(p =>
    p.PerEm == username &&
    (p.PerRole == "ARTIST" || p.PerRole == "MANAGER" || p.PerRole == "CUSTOMER")
);


                if (user != null)
                {
                    HttpContext.Session.SetString("PerId", user.PerId);
                    HttpContext.Session.SetString("PerRole", user.PerRole);
                    HttpContext.Session.SetString("PerEm", user.PerEm);
                    if (user.PerRole == "CUSTOMER")
    return RedirectToAction("MyInvoices", "Fature");

return RedirectToAction("Index", "Home");

                }

                ViewBag.Error = "Nuk ekziston asnjë ARTIST/MANAGER me këtë emër.";
                return View();
            }

            ViewBag.Error = "Username ose password gabim";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
