using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PROJEKTDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

       public async Task<IActionResult> Index()
{
    var role = HttpContext.Session.GetString("PerRole");
    if (string.IsNullOrEmpty(role))
        return RedirectToAction("Login", "Account");

    if (role == "ADMIN")
        return View(); // admin pa model

    if (role == "ARTIST")
    {
        var perId = HttpContext.Session.GetString("PerId");
        if (string.IsNullOrEmpty(perId))
            return RedirectToAction("Login", "Account");

        var pikturat = await _db.Piktures
            .Where(p => p.PerId == perId)
            .Select(p => new ArtistPiktureRowVM
            {
                PikId = p.PikId,
                PikTit = p.PikTit,
                PikCmim = p.PikCmim,

                ShiturSasi = _db.Rresht
                    .Where(r => r.PikId == p.PikId)
                    .Sum(r => (int?)r.RreSasi) ?? 0,

                Xhiro = _db.Rresht
                    .Where(r => r.PikId == p.PikId)
                    .Sum(r => (decimal?)r.RreCmim * r.RreSasi) ?? 0m
            })
            .OrderByDescending(x => x.Xhiro)
            .ToListAsync();

        var vm = new ArtistDashboardVM
        {
            PerId = perId,
            ArtistEmri = HttpContext.Session.GetString("PerEm") ?? "",
            TotalXhiro = pikturat.Sum(x => x.Xhiro),
            Pikturat = pikturat
        };

        return View(vm);
    }

    return View();
}
}
}


