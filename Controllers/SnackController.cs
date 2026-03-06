using Microsoft.AspNetCore.Mvc;
using SnackMVCApp.Models;

namespace SnackMVCApp.Controllers
{
    public class SnacksController : Controller
    {
        private static List<Snack> snacks = new List<Snack>()
        {
            new Snack{
                Id = 1,
                Name = "TopDeck",
                Brand = "Cadbury",
                Type = SnackType.Chocolate,
                Description = "Layers of white and milk chocolate",
                Rating = 5,
                Price = 18
            },
            new Snack{
                Id = 2,
                Name = "Bubble Tea",
                Brand = "KungFu Tea",
                Type = SnackType.Drink,
                Description = "Sweet tea with tapioca pearls",
                Rating = 4,
                Price = 35
            }
        };

        // READ
        public IActionResult Index()
        {
            return View(snacks);
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var snack = snacks.FirstOrDefault(s => s.Id == id);

            if (snack == null)
                return NotFound();

            return View(snack);
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Snack snack)
        {
            if (ModelState.IsValid)
            {
                snack.Id = snacks.Max(s => s.Id) + 1;
                snacks.Add(snack);

                return RedirectToAction("Index");
            }

            return View(snack);
        }

        // EDIT
        public IActionResult Edit(int id)
        {
            var snack = snacks.FirstOrDefault(s => s.Id == id);

            if (snack == null)
                return NotFound();

            return View(snack);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Snack snack)
        {
            if (ModelState.IsValid)
            {
                var existingSnack = snacks.FirstOrDefault(s => s.Id == snack.Id);

                if (existingSnack == null)
                    return NotFound();

                existingSnack.Name = snack.Name;
                existingSnack.Brand = snack.Brand;
                existingSnack.Type = snack.Type;
                existingSnack.Description = snack.Description;
                existingSnack.Rating = snack.Rating;
                existingSnack.Price = snack.Price;

                return RedirectToAction(nameof(Index));
            }

            return View(snack);
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var snack = snacks.FirstOrDefault(s => s.Id == id);

            if (snack == null)
                return NotFound();

            return View(snack);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var snack = snacks.FirstOrDefault(s => s.Id == id);

            snacks.Remove(snack);

            return RedirectToAction("Index");
        }
    }
}