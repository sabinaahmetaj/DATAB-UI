using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;

namespace PROJEKTDB.Controllers
{
    public class FatureController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FatureController(ApplicationDbContext context)
        {
            _context = context;
        }

        /* ================= INDEX ================= */
        public async Task<IActionResult> Index()
        {
            var list = await _context.Fature
                .Include(f => f.Person)
                .Include(f => f.Rresht)
                    .ThenInclude(r => r.Pikture)
                .OrderByDescending(f => f.FatDat)
                .ToListAsync();

            return View(list);
        }

        /* ================= CREATE (GET) ================= */
        public async Task<IActionResult> Create()
        {
            ViewBag.Persons = new SelectList(
                await _context.Persons.OrderBy(p => p.PerEm).ToListAsync(),
                "PerId", "PerEm"
            );

            ViewBag.Piktures = new SelectList(
                await _context.Piktures.OrderBy(p => p.PikTit).ToListAsync(),
                "PikId", "PikTit"
            );

            return View(new FatureCreateVM());
        }

        /* ================= CREATE (POST) ================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FatureCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Persons = new SelectList(_context.Persons, "PerId", "PerEm", vm.PerId);
                ViewBag.Piktures = new SelectList(_context.Piktures, "PikId", "PikTit", vm.PikId);
                return View(vm);
            }

            // 1️⃣ krijo FATURE
            var fature = new Fature
            {
                FatId = vm.FatId,
                PerId = vm.PerId,
                FatDat = vm.FatDat == default ? DateTime.Now : vm.FatDat
            };

            _context.Fature.Add(fature);

            // 2️⃣ merr pikturën (me artist)
            var pikture = await _context.Piktures
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.PikId == vm.PikId);

            if (pikture == null)
            {
                ModelState.AddModelError("", "Piktura nuk u gjet.");
                ViewBag.Persons = new SelectList(_context.Persons, "PerId", "PerEm", vm.PerId);
                ViewBag.Piktures = new SelectList(_context.Piktures, "PikId", "PikTit", vm.PikId);
                return View(vm);
            }

            // 3️⃣ krijo RRESHT (rreshti i parë)
            var rresht = new Rresht
            {
                FatId = vm.FatId,
                RreId = 1,
                PikId = pikture.PikId,
                RreSasi = vm.RreSasi,
                RreCmim = pikture.PikCmim
            };

            _context.Rresht.Add(rresht);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /* ================= AJAX: INFO PIKTURE ================= */
        [HttpGet]
        public async Task<IActionResult> GetPiktureInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var p = await _context.Piktures
                .Include(x => x.Person)
                .FirstOrDefaultAsync(x => x.PikId == id);

            if (p == null) return NotFound();

            return Json(new
            {
                titull = p.PikTit,
                cmim = p.PikCmim,
                artist = p.Person != null
                    ? p.Person.PerEm + " " + p.Person.PerMb
                    : p.PerId
            });
        }

        /* ================= DETAILS ================= */
        public async Task<IActionResult> Details(int id)
        {
            var fature = await _context.Fature
                .Include(f => f.Person)
                .Include(f => f.Rresht)
                    .ThenInclude(r => r.Pikture)
                .FirstOrDefaultAsync(f => f.FatId == id);

            if (fature == null) return NotFound();

            var total = fature.Rresht.Sum(r => (decimal)r.RreSasi * r.RreCmim);

            ViewBag.Rreshtat = fature.Rresht.OrderBy(r => r.RreId).ToList();
            ViewBag.Total = total;

            return View(fature);
        }

        /* ================= DELETE ================= */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fature = await _context.Fature.FindAsync(id);
            if (fature != null)
            {
                _context.Fature.Remove(fature);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
