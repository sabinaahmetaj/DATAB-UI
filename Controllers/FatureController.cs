using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;              // ✅ SHTO KËTË
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;
using PROJEKTDB.Services;


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
        private bool IsCustomer() => HttpContext.Session.GetString("PerRole") == "CUSTOMER";
private string? CurrentPerId() => HttpContext.Session.GetString("PerId");


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

        public async Task<IActionResult> MyInvoices()
{
    if (!IsCustomer())
        return RedirectToAction("Index", "Home");

    var perId = CurrentPerId();
    if (string.IsNullOrWhiteSpace(perId))
        return RedirectToAction("Login", "Account");

    var list = await _context.Fature
        .Where(f => f.PerId == perId)
        .Include(f => f.Person)
        .Include(f => f.Rresht)
            .ThenInclude(r => r.Pikture)
        .OrderByDescending(f => f.FatDat)
        .AsNoTracking()
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
            var isCustomer = await _context.Persons
                .AnyAsync(p => p.PerId == vm.PerId && p.PerRole == "CUSTOMER");

            if (!isCustomer)
            {
                ModelState.AddModelError(nameof(vm.PerId), "Duhet të zgjedhësh vetëm klient (CUSTOMER).");
                await LoadCreateDropdowns(vm.PerId, vm.PikId);
                return View(vm);
            }

            // 1) merr pikturën (çmimi merret nga DB)
            var pikture = await _context.Piktures
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PikId == vm.PikId);

            if (pikture == null)
            {
                ModelState.AddModelError(nameof(vm.PikId), "Piktura nuk u gjet.");
                await LoadCreateDropdowns(vm.PerId, vm.PikId);
                return View(vm);
            }

            // 2) krijo FATURE (PA FatId sepse është IDENTITY)
    // 2) krijo FATURE (PA FatId sepse është IDENTITY)
var fature = new Fature
{
    PerId = vm.PerId,
    FatDat = vm.FatDat == default ? DateTime.Now : vm.FatDat
};

// ✅ TRANSACTION: mos të prishet logjika, por të mos mbetet fatura pa rresht
await using var tx = await _context.Database.BeginTransactionAsync();

try
{
    _context.Fature.Add(fature);

    // ✅ SaveChanges #1 -> DB gjeneron FatId
    await _context.SaveChangesAsync();

    // 3) krijo RRESHT (rreshti i parë) me FatId të gjeneruar
    var rresht = new Rresht
    {
        FatId = fature.FatId,      // ✅ ID i gjeneruar
        RreId = 1,
        PikId = pikture.PikId,
        RreSasi = vm.RreSasi,
        RreCmim = pikture.PikCmim  // ✅ çmimi automatik nga piktura
    };

    _context.Rresht.Add(rresht);

    // ✅ SaveChanges #2 -> ruan rreshtin
    await _context.SaveChangesAsync();

    await tx.CommitAsync();
    return RedirectToAction(nameof(Index));
}
catch
{
    await tx.RollbackAsync();
    throw; // e lë error-in të shfaqet siç e menaxhon aplikacioni yt
}
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

public async Task<IActionResult> DetailsCustomer(int id)
{
    if (!IsCustomer())
        return RedirectToAction("Index", "Home");

    var perId = CurrentPerId();
    if (string.IsNullOrWhiteSpace(perId))
        return RedirectToAction("Login", "Account");

    var fature = await _context.Fature
        .Include(f => f.Person)
        .Include(f => f.Rresht)
            .ThenInclude(r => r.Pikture)
        .AsNoTracking()
        .FirstOrDefaultAsync(f => f.FatId == id && f.PerId == perId);

    if (fature == null) return NotFound();

    // total për view
    ViewBag.Total = fature.Rresht.Sum(r => (decimal)r.RreSasi * r.RreCmim);

    return View("Invoice", fature); // ✅ view e re “Invoice.cshtml” që duket si faturë reale
}

[HttpGet]
public async Task<IActionResult> Pdf(int id)
{
    // Admin/Manager lejohet
    // Customer lejohet vetëm për faturat e veta
    var role = HttpContext.Session.GetString("PerRole");
    var perId = HttpContext.Session.GetString("PerId");

    if (role != "ADMIN" && role != "MANAGER" && role != "CUSTOMER")
        return RedirectToAction("Index", "Home");

    var query = _context.Fature
        .Include(f => f.Person)
        .Include(f => f.Rresht)
            .ThenInclude(r => r.Pikture)
        .AsQueryable();

    if (role == "CUSTOMER")
    {
        if (string.IsNullOrWhiteSpace(perId))
            return RedirectToAction("Login", "Account");

        query = query.Where(f => f.PerId == perId);
    }

    var fature = await query.FirstOrDefaultAsync(f => f.FatId == id);
    if (fature == null) return NotFound();

    var bytes = InvoicePdfService.Generate(fature);
    return File(bytes, "application/pdf", $"Fature_{fature.FatId}.pdf");
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
            if (fature == null) return NotFound();

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
