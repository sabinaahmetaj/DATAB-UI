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

        private bool IsAdmin() => HttpContext.Session.GetString("PerRole") == "ADMIN";
        private bool IsManager() => HttpContext.Session.GetString("PerRole") == "MANAGER";
        private bool IsAdminOrManager() => IsAdmin() || IsManager();

        /* ================= INDEX ================= */
        public async Task<IActionResult> Index()
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            var list = await _context.Fature
                .Include(f => f.Person)
                .Include(f => f.Rresht)
                    .ThenInclude(r => r.Pikture)
                .OrderByDescending(f => f.FatDat)
                .ToListAsync();

            return View(list);
        }

        /* ================= helpers (dropdowns) ================= */
        private async Task LoadCreateDropdowns(string? selectedPerId = null, string? selectedPikId = null)
        {
            // ✅ VETËM CUSTOMER
            var customers = await _context.Persons
                .Where(p => p.PerRole == "CUSTOMER")
                .Select(p => new
                {
                    p.PerId,
                    FullName = p.PerEm + " " + p.PerMb
                })
                .OrderBy(x => x.FullName)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Persons = new SelectList(customers, "PerId", "FullName", selectedPerId);

            var piktures = await _context.Piktures
                .Select(p => new
                {
                    p.PikId,
                    p.PikTit
                })
                .OrderBy(p => p.PikTit)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.Piktures = new SelectList(piktures, "PikId", "PikTit", selectedPikId);
        }

        /* ================= CREATE (GET) ================= */
        public async Task<IActionResult> Create()
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            await LoadCreateDropdowns();
            return View(new FatureCreateVM
            {
                FatDat = DateTime.Now,
                RreSasi = 1
            });
        }

        /* ================= CREATE (POST) ================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FatureCreateVM vm)
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
            {
                await LoadCreateDropdowns(vm.PerId, vm.PikId);
                return View(vm);
            }

            // ✅ kontrollo që personi i zgjedhur të jetë CUSTOMER
            var isCustomer = await _context.Persons.AnyAsync(p => p.PerId == vm.PerId && p.PerRole == "CUSTOMER");
            if (!isCustomer)
            {
                ModelState.AddModelError(nameof(vm.PerId), "Duhet të zgjedhësh vetëm klient (CUSTOMER).");
                await LoadCreateDropdowns(vm.PerId, vm.PikId);
                return View(vm);
            }

            // 1) krijo FATURE
            var fature = new Fature
            {
                FatId = vm.FatId,
                PerId = vm.PerId,
                FatDat = vm.FatDat == default ? DateTime.Now : vm.FatDat
            };
            _context.Fature.Add(fature);

            // 2) merr pikturën (vetëm çmimin)
            var pikture = await _context.Piktures
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PikId == vm.PikId);

            if (pikture == null)
            {
                ModelState.AddModelError(nameof(vm.PikId), "Piktura nuk u gjet.");
                await LoadCreateDropdowns(vm.PerId, vm.PikId);
                return View(vm);
            }

            // 3) krijo RRESHT (rreshti i parë)
            var rresht = new Rresht
            {
                FatId = vm.FatId,
                RreId = 1,
                PikId = pikture.PikId,
                RreSasi = vm.RreSasi,
                RreCmim = pikture.PikCmim // ✅ çmimi automatik nga piktura
            };
            _context.Rresht.Add(rresht);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /* ================= AJAX: INFO PIKTURE ================= */
        [HttpGet]
        public async Task<IActionResult> GetPiktureInfo(string id)
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var info = await _context.Piktures
                .Where(p => p.PikId == id)
                .Select(p => new
                {
                    cmim = p.PikCmim,
                    artist = _context.Persons
                        .Where(per => per.PerId == p.PerId)
                        .Select(per => per.PerEm + " " + per.PerMb)
                        .FirstOrDefault()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (info == null) return NotFound();

            return Json(new
            {
                cmim = info.cmim,
                artist = info.artist ?? ""
            });
        }

        /* ================= DETAILS ================= */
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

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
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            var fature = await _context.Fature.FindAsync(id);
            if (fature != null)
            {
                _context.Fature.Remove(fature);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /* ================= EDIT (GET) ================= */
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            var fature = await _context.Fature.FindAsync(id);
            if (fature == null) return NotFound(); // ✅ kjo heq CS8602

            var customers = await _context.Persons
                .Where(p => p.PerRole == "CUSTOMER")
                .Select(p => new { p.PerId, FullName = p.PerEm + " " + p.PerMb })
                .OrderBy(x => x.FullName)
                .ToListAsync();

            ViewBag.Persons = new SelectList(customers, "PerId", "FullName", fature.PerId);
            return View(fature);
        }

        /* ================= EDIT (POST) ================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fature model)
        {
            if (!IsAdminOrManager())
                return RedirectToAction("Index", "Home");

            if (id != model.FatId) return BadRequest();

            if (!ModelState.IsValid)
            {
                var customers = await _context.Persons
                    .Where(p => p.PerRole == "CUSTOMER")
                    .Select(p => new { p.PerId, FullName = p.PerEm + " " + p.PerMb })
                    .OrderBy(x => x.FullName)
                    .ToListAsync();

                ViewBag.Persons = new SelectList(customers, "PerId", "FullName", model.PerId);
                return View(model);
            }

            var fature = await _context.Fature.FindAsync(id);
            if (fature == null) return NotFound();

            fature.PerId = model.PerId;
            fature.FatDat = model.FatDat;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
