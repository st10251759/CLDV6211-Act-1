using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnackMVCApp.Data;
using SnackMVCApp.Models;
using SnackMVCApp.Services;

namespace SnackMVCApp.Controllers
{
    public class SnacksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public SnacksController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // READ - Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Snacks.ToListAsync());
        }

        // DETAILS
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
                // Upload image to Azurite if file was selected
                if (snack.ImageFile != null && snack.ImageFile.Length > 0)
                {
                    snack.ImageUrl = await _blobService.UploadAsync(snack.ImageFile);
                }

                _context.Add(snack);
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
                    // If a new image was uploaded, delete old blob and upload new one
                    if (snack.ImageFile != null && snack.ImageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(snack.ImageUrl))
                            await _blobService.DeleteAsync(snack.ImageUrl);

                        snack.ImageUrl = await _blobService.UploadAsync(snack.ImageFile);
                    }

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

        // DELETE - GET
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
                // Delete image from Azurite blob storage
                if (!string.IsNullOrEmpty(snack.ImageUrl))
                    await _blobService.DeleteAsync(snack.ImageUrl);

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
