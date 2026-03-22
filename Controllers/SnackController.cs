using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnackMVCApp.Data;
using SnackMVCApp.Models;
using SnackMVCApp.Services;

namespace SnackMVCApp.Controllers
{
    // Main controller for CRUD operations on Snacks
    // Uses EF Core for database + BlobService for image storage
    public class SnacksController : Controller
    {
        // Database context - injected by ASP.NET Dependency Injection
        private readonly ApplicationDbContext _context;

        // Blob storage service - handles image upload/delete to Azurite
        private readonly BlobService _blobService;

        // Constructor - ASP.NET automatically provides both services
        public SnacksController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // 📋 READ - Index: Shows ALL snacks in a table
        // GET /Snacks → Snack/Index.cshtml
        public async Task<IActionResult> Index()
        {
            // Async query gets all snacks from SQL database
            var snacks = await _context.Snacks.ToListAsync();
            return View(snacks);
        }

        // 👁️ READ - Details: Shows ONE snack with image
        // GET /Snacks/Details/5 → Snack/Details.cshtml
        public async Task<IActionResult> Details(int? id)
        {
            // Validate ID exists
            if (id == null) return NotFound(); // Returns 404

            // Find snack by ID from database (async)
            var snack = await _context.Snacks.FirstOrDefaultAsync(s => s.Id == id);

            if (snack == null) return NotFound(); // Not found in DB

            return View(snack); // Pass snack to Details view
        }

        // ➕ CREATE - GET: Shows empty form
        // GET /Snacks/Create → Snack/Create.cshtml
        public IActionResult Create()
        {
            // No async needed - just show empty form
            return View();
        }

        // ➕ CREATE - POST: Saves new snack + uploads image
        // POST /Snacks/Create → Redirect to Index
        [HttpPost] // Only responds to form POST requests
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> Create(Snack snack)
        {
            // ASP.NET validates model (Required, Range, etc.)
            if (ModelState.IsValid)
            {
                // 🚀 UPLOAD IMAGE: If user selected a file
                if (snack.ImageFile != null && snack.ImageFile.Length > 0)
                {
                    // Uploads to Azurite → returns public URL
                    snack.ImageUrl = await _blobService.UploadAsync(snack.ImageFile);
                }

                // Add snack to database (EF auto-generates ID)
                _context.Add(snack);

                // Save to SQL database
                await _context.SaveChangesAsync();

                // Success! Redirect to avoid duplicate form resubmit
                return RedirectToAction(nameof(Index));
            }

            // Validation failed → return form with errors
            return View(snack);
        }

        // ✏️ EDIT - GET: Shows form with existing data
        // GET /Snacks/Edit/5 → Snack/Edit.cshtml
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Find existing snack by ID
            var snack = await _context.Snacks.FindAsync(id);
            if (snack == null) return NotFound();

            return View(snack); // Pre-fill form with snack data
        }

        // ✏️ EDIT - POST: Updates snack + handles image replacement
        // POST /Snacks/Edit/5 → Redirect to Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Snack snack)
        {
            // Security: Ensure user didn't change the ID
            if (id != snack.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 🖼️ IMAGE HANDLING: Replace image if new file uploaded
                    if (snack.ImageFile != null && snack.ImageFile.Length > 0)
                    {
                        // Delete OLD image from Azurite
                        if (!string.IsNullOrEmpty(snack.ImageUrl))
                            await _blobService.DeleteAsync(snack.ImageUrl);

                        // Upload NEW image to Azurite
                        snack.ImageUrl = await _blobService.UploadAsync(snack.ImageFile);
                    }
                    // If no new image → keep existing ImageUrl (thanks to hidden field)

                    // Update snack in database
                    _context.Update(snack);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Optimistic concurrency conflict
                    if (!SnackExists(snack.Id)) return NotFound();
                    throw; // Let ASP.NET handle it
                }
            }

            // Validation failed → return form with errors
            return View(snack);
        }

        // 🗑️ DELETE - GET: Confirm deletion with preview
        // GET /Snacks/Delete/5 → Snack/Delete.cshtml
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var snack = await _context.Snacks.FirstOrDefaultAsync(s => s.Id == id);
            if (snack == null) return NotFound();

            return View(snack); // Shows snack details + "Are you sure?"
        }

        // 🗑️ DELETE - POST: Permanently deletes snack + image
        // POST /Snacks/Delete/5 → Redirect to Index
        [HttpPost, ActionName("Delete")] // Maps to Delete POST but calls DeleteConfirmed
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var snack = await _context.Snacks.FindAsync(id);
            if (snack != null)
            {
                // 🖼️ DELETE IMAGE: Remove from Azurite blob storage
                if (!string.IsNullOrEmpty(snack.ImageUrl))
                    await _blobService.DeleteAsync(snack.ImageUrl);

                // Remove from SQL database
                _context.Snacks.Remove(snack);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper: Checks if snack exists in database
        private bool SnackExists(int id)
        {
            return _context.Snacks.Any(e => e.Id == id);
        }
    }
}
