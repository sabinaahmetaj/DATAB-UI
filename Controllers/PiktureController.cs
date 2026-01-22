using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;

namespace PROJEKTDB.Controllers
{
    public class PiktureController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PiktureController(ApplicationDbContext context)
        {
            _context = context;
        }

private bool IsAdmin() => HttpContext.Session.GetString("PerRole") == "ADMIN";
private bool IsManager() => HttpContext.Session.GetString("PerRole") == "MANAGER";
private bool IsAdminOrManager() => IsAdmin() || IsManager();

        // GET: /Pikture
        public async Task<IActionResult> Index()
        {
            var data = await _context.Piktures
                .Include(p => p.Person)
                .Include(p => p.Galeri)
                .ToListAsync();

            return View(data);
        }

        // GET: /Pikture/Details/0001
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var pikture = await _context.Piktures
                .Include(p => p.Person)
                .Include(p => p.Galeri)
                .FirstOrDefaultAsync(x => x.PikId == id);

            if (pikture == null)
                return NotFound();

            return View(pikture);
        }

        // GET: /Pikture/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();   // nuk zgjedh asgjë automatikisht
            return View();
        }

        // POST: /Pikture/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pikture pikture)
        {
            // nëse ke [Required] te model, këtu do të kapet automatikisht kur GalId/PerId janë bosh
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(pikture.PerId, pikture.GalId);
                return View(pikture);
            }

            _context.Piktures.Add(pikture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Pikture/Edit/0001
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var pikture = await _context.Piktures.FindAsync(id);
            if (pikture == null)
                return NotFound();

            await LoadDropdowns(pikture.PerId, pikture.GalId);
            return View(pikture);
        }

        // POST: /Pikture/Edit/0001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Pikture pikture)
        {
            if (id != pikture.PikId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(pikture.PerId, pikture.GalId);
                return View(pikture);
            }

            try
            {
                _context.Update(pikture);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Piktures.AnyAsync(x => x.PikId == id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Pikture/Delete/0001
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var pikture = await _context.Piktures
                .Include(p => p.Person)
                .Include(p => p.Galeri)
                .FirstOrDefaultAsync(x => x.PikId == id);

            if (pikture == null)
                return NotFound();

            return View(pikture);
        }

        // POST: /Pikture/Delete/0001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var pikture = await _context.Piktures.FindAsync(id);
            if (pikture != null)
            {
                _context.Piktures.Remove(pikture);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Mbush dropdown-at për Person dhe Galeri.
        /// selectedPerId / selectedGalId përdoren për Edit ose kur Create kthehet me error.
        /// </summary>
        private async Task LoadDropdowns(string? selectedPerId = null, string? selectedGalId = null)
        {
          var persons = await _context.Persons
    .Where(x => x.PerRole == "ARTIST")   // ✅ vetëm artistë
    .Select(x => new
    {
        x.PerId,
        Text = x.PerEm + " " + x.PerMb + " (" + x.PerId + ")"
    })
    .ToListAsync();


            var galeris = await _context.Galeris
                .Select(x => new
                {
                    x.GalId,
                    Text = x.GalMng + " (" + x.GalId + ")"
                })
                .ToListAsync();

            // Këto përdoren te Create/Edit view
            ViewBag.Persons = new SelectList(persons, "PerId", "Text", selectedPerId);
            ViewBag.Galeris = new SelectList(galeris, "GalId", "Text", selectedGalId);
        }
    }
}
