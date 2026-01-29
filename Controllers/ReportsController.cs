using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PROJEKTDB.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // =====================================================
        // RAPORT: LISTA E FATURAVE + TOTALI PER CDO GALERI
        // ACCESS: ADMIN / MANAGER (me Session)
        // URL: /Reports/GalleryInvoices
        // =====================================================
        public async Task<IActionResult> GalleryInvoices()
        {
            var role = HttpContext.Session.GetString("PerRole");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            if (role != "ADMIN" && role != "MANAGER")
                return Forbid();

            // -----------------------------------------------------
            // 1) LISTA E FATURAVE SIPAS GALERISE
            // (Nëse 1 faturë ka piktura nga disa galeri, del te secila)
            // -----------------------------------------------------
            var invoicesPerGallery = await (
                from f in _db.Fature
                join r in _db.Rresht on f.FatId equals r.FatId
                join p in _db.Piktures on r.PikId equals p.PikId
                join g in _db.Galeris on p.GalId equals g.GalId
                group new { f, r, g } by new
                {
                    g.GalId,
                    GalName = g.GalEm,     // ✅ Emri i galerisë
                    f.FatId,
                    f.FatDat
                }
                into grp
                select new GalleryInvoiceRowVM
                {
                    GalId = grp.Key.GalId,
                    GalName = grp.Key.GalName,
                    FatId = grp.Key.FatId,
                    FatDat = grp.Key.FatDat,
                    TotalInThisGallery = grp.Sum(x => x.r.RreSasi * x.r.RreCmim)
                }
            )
            .OrderBy(x => x.GalId)
            .ThenByDescending(x => x.FatDat)
            .ThenByDescending(x => x.FatId)
            .ToListAsync();

            // -----------------------------------------------------
            // 2) TOTALI I PERGJITHSHEM I SHITJEVE PER CDO GALERI
            // -----------------------------------------------------
            var totalsPerGallery = await (
                from r in _db.Rresht
                join p in _db.Piktures on r.PikId equals p.PikId
                join g in _db.Galeris on p.GalId equals g.GalId
                group r by new
                {
                    g.GalId,
                    GalName = g.GalEm      // ✅ Emri i galerisë (jo menaxheri)
                }
                into grp
                select new GalleryTotalVM
                {
                    GalId = grp.Key.GalId,
                    GalName = grp.Key.GalName,
                    TotalSales = grp.Sum(x => x.RreSasi * x.RreCmim)
                }
            )
            .OrderByDescending(x => x.TotalSales)
            .ToListAsync();

            var vm = new GalleryInvoicesReportVM
            {
                InvoicesPerGallery = invoicesPerGallery,
                TotalsPerGallery = totalsPerGallery
            };

            return View(vm);
        }

        // =====================================================
        // RAPORT: LISTA E TE GJITHA PIKTURAVE (VETEM MANAGER)
        // URL: /Reports/AllPikture
        // =====================================================
        public async Task<IActionResult> AllPikture()
        {
            var role = HttpContext.Session.GetString("PerRole");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "Account");

            if (role != "MANAGER")
                return Forbid();

            // ✅ Pa Include - sepse te projekti yt JOIN është më i sigurt
            var rows = await (
                from p in _db.Piktures
                join g in _db.Galeris on p.GalId equals g.GalId
                join per in _db.Persons on p.PerId equals per.PerId
                select new ManagerPiktureRowVM
                {
                    PikId = p.PikId,
                    PikTit = p.PikTit,
                    PikCmim = p.PikCmim,

                    GalId = g.GalId,
                    GalEm = g.GalEm,

                    ArtistId = per.PerId,
                    ArtistEm = per.PerEm,

                    ShiturSasi = _db.Rresht
                        .Where(r => r.PikId == p.PikId)
                        .Sum(r => (int?)r.RreSasi) ?? 0,

                    Xhiro = _db.Rresht
                        .Where(r => r.PikId == p.PikId)
                        .Sum(r => (decimal?)r.RreCmim * r.RreSasi) ?? 0m
                }
            )
            .OrderByDescending(x => x.Xhiro)
            .ToListAsync();

            var vm = new ManagerPiktureReportVM
            {
                TotalPiktura = rows.Count,
                TotalShiturSasi = rows.Sum(x => x.ShiturSasi),
                TotalXhiro = rows.Sum(x => x.Xhiro),
                Rows = rows
            };

            return View(vm);
        }
    }
}
