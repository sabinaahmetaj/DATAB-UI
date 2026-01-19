using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;

namespace PROJEKTDB.Controllers
{
    public class GaleriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GaleriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Galeris
        public async Task<IActionResult> Index()
        {
            return View(await _context.Galeris.ToListAsync());
        }

        // GET: /Galeris/Details/001
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var galeri = await _context.Galeris.FirstOrDefaultAsync(x => x.GalId == id);
            if (galeri == null) return NotFound();

            return View(galeri);
        }

        // GET: /Galeris/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Galeris/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Galeri galeri)
        {
            if (!ModelState.IsValid) return View(galeri);

            _context.Add(galeri);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Galeris/Edit/001
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var galeri = await _context.Galeris.FindAsync(id);
            if (galeri == null) return NotFound();

            return View(galeri);
        }

        // POST: /Galeris/Edit/001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Galeri galeri)
        {
            if (id != galeri.GalId) return NotFound();
            if (!ModelState.IsValid) return View(galeri);

            try
            {
                _context.Update(galeri);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Galeris.AnyAsync(x => x.GalId == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Galeris/Delete/001
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var galeri = await _context.Galeris.FirstOrDefaultAsync(x => x.GalId == id);
            if (galeri == null) return NotFound();

            return View(galeri);
        }

        // POST: /Galeris/Delete/001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var galeri = await _context.Galeris.FindAsync(id);
            if (galeri != null)
            {
                _context.Galeris.Remove(galeri);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
