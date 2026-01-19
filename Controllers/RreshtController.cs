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

        // GET: /Rresht/Create?fatId=111111
        public async Task<IActionResult> Create(int? fatId)
        {
            ViewBag.Fature = new SelectList(
                await _context.Fature.OrderByDescending(f => f.FatDat).ToListAsync(),
                "FatId", "FatId", fatId
            );

            ViewBag.Pikture = new SelectList(
                await _context.Piktures.OrderBy(p => p.PikTit).ToListAsync(),
                "PikId", "PikTit"
            );

            var model = new Rresht();
            if (fatId.HasValue) model.FatId = fatId.Value;

            return View(model);
        }

        // POST: /Rresht/Create
       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Rresht rresht)
{
    // dropdowns / data që duhet në View kur ka error:
    async Task LoadDropdowns()
    {
        ViewBag.Fature = new SelectList(await _context.Fature.ToListAsync(), "FatId", "FatId", rresht.FatId);
        ViewBag.Pikture = new SelectList(await _context.Piktures.ToListAsync(), "PikId", "PikTitull", rresht.PikId);
    }

    if (!ModelState.IsValid)
    {
        await LoadDropdowns();
        return View(rresht);
    }

    // nëse s’është dhënë RreId, gjeneroje automatikisht (tinyint -> byte)
    if (rresht.RreId == 0)
    {
        var maxRreId = await _context.Rresht
            .Where(x => x.FatId == rresht.FatId)
            .Select(x => (byte?)x.RreId)
            .MaxAsync();

        var next = (byte)((maxRreId ?? 0) + 1);
        rresht.RreId = next;
    }

    _context.Add(rresht);
    await _context.SaveChangesAsync();

    // kthehu te lista e rreshtave të asaj fature
    return RedirectToAction(nameof(Index), new { fatId = rresht.FatId });
}


        

        // GET: /Rresht/Edit?fatId=111111&rreId=1
// GET: /Rresht/Edit?fatId=...&rreId=...
public async Task<IActionResult> Edit(int fatId, byte rreId)
{
    var rresht = await _context.Rresht.FindAsync(fatId, rreId);
    if (rresht == null) return NotFound();

    // (nese ke dropdowns, mbushi ketu)
     ViewBag.Piktures = new SelectList(_context.Piktures, "PikId", "PikTit", rresht.PikId);

    return View(rresht);
}



        // POST: /Rresht/Edit
       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int fatId, byte rreId, Rresht rresht)
{
    if (fatId != rresht.FatId || rreId != rresht.RreId) return NotFound();

    if (!ModelState.IsValid)
    {
        // rimbushe dropdowns nese i ke
        return View(rresht);
    }

    _context.Update(rresht);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index), new { fatId = fatId });
}


        // POST: /Rresht/Delete
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
