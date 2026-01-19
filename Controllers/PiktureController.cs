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

        // GET: /Piktures
        public async Task<IActionResult> Index()
        {
            var data = await _context.Piktures
                .Include(p => p.Person)
                .Include(p => p.Galeri)
                .ToListAsync();

            return View(data);
        }

        // GET: /Piktures/Details/0001
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var pikture = await _context.Piktures
                .Include(p => p.Person)
                .Include(p => p.Galeri)
                .FirstOrDefaultAsync(x => x.PikId == id);

            if (pikture == null) return NotFound();

            return View(pikture);
        }

        // GET: /Piktures/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // POST: /Piktures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pikture pikture)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(pikture.PerId, pikture.GalId);
                return View(pikture);
            }

            _context.Add(pikture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Piktures/Edit/0001
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var pikture = await _context.Piktures.FindAsync(id);
            if (pikture == null) return NotFound();

            await LoadDropdowns(pikture.PerId, pikture.GalId);
            return View(pikture);
        }

        // POST: /Piktures/Edit/0001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Pikture pikture)
        {
            if (id != pikture.PikId) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(pikture.PerId, pikture.GalId);
                return View(pikture);
            }

            _context.Update(pikture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Piktures/Delete/0001
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var pikture = await _context.Piktures
                .Include(p => p.Person)
                .Include(p => p.Galeri)
                .FirstOrDefaultAsync(x => x.PikId == id);

            if (pikture == null) return NotFound();

            return View(pikture);
        }

        // POST: /Piktures/Delete/0001
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

        private async Task LoadDropdowns(string? selectedPerId = null, string? selectedGalId = null)
        {
            var persons = await _context.Persons
                .Select(x => new { x.PerId, Text = x.PerEm + " " + x.PerMb + " (" + x.PerId + ")" })
                .ToListAsync();

            var galeris = await _context.Galeris
                .Select(x => new { x.GalId, Text = x.GalMng + " (" + x.GalId + ")" })
                .ToListAsync();

            ViewBag.Persons = new SelectList(persons, "PerId", "Text", selectedPerId);
            ViewBag.Galeris = new SelectList(galeris, "GalId", "Text", selectedGalId);
        }
    }
}
