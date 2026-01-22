using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Data;
using PROJEKTDB.Models;

namespace PROJEKTDB.Controllers
{
    public class PersonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("PerRole") == "ADMIN";
        }

        // GET: /Persons
       public async Task<IActionResult> Index()
{
    if (!IsAdmin())
        return RedirectToAction("Index", "Home");

    var persons = await _context.Persons.ToListAsync();
    return View(persons);
}


        // GET: /Persons/Create
       public IActionResult Create()
{
    if (!IsAdmin())
        return RedirectToAction("Index", "Home");

    return View(new Person { PerRole = "CUSTOMER" });
}


        // POST: /Persons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]      
public async Task<IActionResult> Create(Person person)
{
    if (!IsAdmin())
        return RedirectToAction("Index", "Home");

    if (!ModelState.IsValid)
        return View(person);

    _context.Persons.Add(person);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}


        // GET: /Persons/Edit/A12345678B
        public async Task<IActionResult> Edit(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
                return NotFound();

            return View(person);
        }

        // POST: /Persons/Edit/A12345678B
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PerId,PerEm,PerMb,PerDat,PerZon,PerTel,PerRole")] Person person)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (id != person.PerId)
                return BadRequest();

            if (string.IsNullOrWhiteSpace(person.PerRole))
                person.PerRole = "CUSTOMER";

            if (!ModelState.IsValid)
                return View(person);

            try
            {
                _context.Update(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Gabim gjatë ruajtjes në databazë: " + ex.Message);
                return View(person);
            }
        }

        // GET: /Persons/Delete/A12345678B
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var person = await _context.Persons.AsNoTracking().FirstOrDefaultAsync(x => x.PerId == id);
            if (person == null)
                return NotFound();

            return View(person);
        }

        // POST: /Persons/Delete/A12345678B
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Home");

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
                return NotFound();

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
