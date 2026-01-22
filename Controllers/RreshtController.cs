using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;

namespace PROJEKTDB.Controllers
{
    public class RreshtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RreshtController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsAdmin() => HttpContext.Session.GetString("PerRole") == "ADMIN";
private bool IsManager() => HttpContext.Session.GetString("PerRole") == "MANAGER";
private bool IsAdminOrManager() => IsAdmin() || IsManager();


        // =========================
        // Helpers: Dropdowns
        // =========================
        private async Task LoadDropdowns(int? selectedFatId = null, string? selectedPikId = null)
        {
            // Fature dropdown (nëse e përdor në view)
            var fatureList = await _context.Fature
                .OrderByDescending(f => f.FatDat)
                .Select(f => new { f.FatId })
                .ToListAsync();

            ViewBag.Fature = new SelectList(fatureList, "FatId", "FatId", selectedFatId);

            // Pikture dropdown
            var piktureList = await _context.Piktures
                .OrderBy(p => p.PikTit)
                .Select(p => new { p.PikId, p.PikTit })
                .ToListAsync();

            // ✅ Emri i saktë i fushës: PikTit (jo PikTitull)
            ViewBag.Piktures = new SelectList(piktureList, "PikId", "PikTit", selectedPikId);
        }

        // ================= INDEX =================
        // /Rresht?fatId=111111
        public async Task<IActionResult> Index(int? fatId)
        {
            var q = _context.Rresht
                .Include(r => r.Fature)
                .Include(r => r.Pikture)
                .AsQueryable();

            if (fatId.HasValue)
                q = q.Where(r => r.FatId == fatId.Value);

            var list = await q
                .OrderBy(r => r.FatId)
                .ThenBy(r => r.RreId)
                .ToListAsync();

            ViewBag.FatId = fatId;
            return View(list);
        }

        // ================= CREATE (GET) =================
        // GET: /Rresht/Create?fatId=111111
        public async Task<IActionResult> Create(int? fatId)
        {
            await LoadDropdowns(fatId, null);

            var model = new Rresht();
            if (fatId.HasValue) model.FatId = fatId.Value;

            return View(model);
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rresht rresht)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(rresht.FatId, rresht.PikId);
                return View(rresht);
            }

            // nëse s’është dhënë RreId, gjeneroje automatikisht
            if (rresht.RreId == 0)
            {
                var maxRreId = await _context.Rresht
                    .Where(x => x.FatId == rresht.FatId)
                    .Select(x => (byte?)x.RreId)
                    .MaxAsync();

                rresht.RreId = (byte)((maxRreId ?? 0) + 1);
            }

            // Sigurohu që s’po shton PK të dyfishtë
            var exists = await _context.Rresht.AnyAsync(x => x.FatId == rresht.FatId && x.RreId == rresht.RreId);
            if (exists)
            {
                ModelState.AddModelError("", "Ky rresht ekziston (FAT_ID + RRE_ID). Provo përsëri.");
                await LoadDropdowns(rresht.FatId, rresht.PikId);
                return View(rresht);
            }

            _context.Rresht.Add(rresht);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { fatId = rresht.FatId });
        }

        // ================= EDIT (GET) =================
        // GET: /Rresht/Edit?fatId=111111&rreId=1
        public async Task<IActionResult> Edit(int fatId, byte rreId)
        {
            var rresht = await _context.Rresht.FindAsync(fatId, rreId);
            if (rresht == null) return NotFound();

            await LoadDropdowns(rresht.FatId, rresht.PikId);
            return View(rresht);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int fatId, byte rreId, Rresht rresht)
        {
            if (fatId != rresht.FatId || rreId != rresht.RreId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(rresht.FatId, rresht.PikId);
                return View(rresht);
            }

            try
            {
                _context.Update(rresht);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Gabim gjatë ruajtjes në DB: " + ex.Message);
                await LoadDropdowns(rresht.FatId, rresht.PikId);
                return View(rresht);
            }

            return RedirectToAction(nameof(Index), new { fatId = fatId });
        }

        // ================= DELETE =================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int fatId, byte rreId)
        {
            var rresht = await _context.Rresht.FindAsync(fatId, rreId);
            if (rresht != null)
            {
                _context.Rresht.Remove(rresht);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { fatId });
        }
    }
}
