using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnackMVCApp.Data;
using SnackMVCApp.Models;

namespace SnackMVCApp.Controllers
{
    public class SnacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor injects DbContext via DI
        public SnacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ - Index (List all)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Snacks.ToListAsync());
        }

        // READ - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var snack = await _context.Snacks.FirstOrDefaultAsync(s => s.Id == id);
            if (snack == null) return NotFound();

            return View(snack);
        }

        // CREATE - GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Snack snack)
        {
            if (ModelState.IsValid)
            {
                _context.Add(snack);  // EF auto-generates Id
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(snack);
        }

        // EDIT - GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var snack = await _context.Snacks.FindAsync(id);
            if (snack == null) return NotFound();

            return View(snack);
        }

        // EDIT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Snack snack)
        {
            if (id != snack.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(snack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SnackExists(snack.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(snack);
        }

        // DELETE - GET (Confirm)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var snack = await _context.Snacks.FirstOrDefaultAsync(s => s.Id == id);
            if (snack == null) return NotFound();

            return View(snack);
        }

        // DELETE - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var snack = await _context.Snacks.FindAsync(id);
            if (snack != null)
            {
                _context.Snacks.Remove(snack);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SnackExists(int id)
        {
            return _context.Snacks.Any(e => e.Id == id);
        }
    }
}
