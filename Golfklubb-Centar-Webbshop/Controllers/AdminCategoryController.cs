using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoryController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        public AdminCategoryController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.Include(c => c.FkParentCategory).ToListAsync(); //Placerar alla kategorier i en lista

            return View(categories);
        }

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _context.Categories
                .Include(c => c.FkParentCategory)
                .Include(c => c.Products)  // Hämtar produkter kopplade till kategorin
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        public IActionResult CategoryCreate()
        {
            var viewModel = new CategoryCreateViewModel
            {
                ParentCategories = _context.Categories.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryCreate(CategoryCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(viewModel.Category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            // Fyll igen om validering failar
            viewModel.ParentCategories = _context.Categories.ToList();
            TempData["Success"] = "Kategorin " + viewModel.Category.CategoryName + " är skapad.";
            return View(viewModel);
        }

        public async Task<IActionResult> CategoryEdit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var viewModel = new CategoryEditViewModel
            {
                Category = category,
                ParentCategories = _context.Categories
                    .Where(c => c.CategoryId != id)  // Kan inte vara sin egen förälder
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit(CategoryEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(viewModel.Category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            viewModel.ParentCategories = _context.Categories
                .Where(c => c.CategoryId != viewModel.Category.CategoryId)
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.InverseFkParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category.Products.Any())
            {
                TempData["Error"] = "Kan inte radera kategorin, den har produkter kopplade till sig.";
                return RedirectToAction("CategoryDetails", new { id });  // skicka tillbaka till detaljsidan
            }

            if (category.InverseFkParentCategory.Any())
            {
                TempData["Error"] = "Kan inte radera kategorin, den har subkategorier kopplade till sig.";
                return RedirectToAction("CategoryDetails", new { id });
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kategorin är raderad.";
            return RedirectToAction("Categories");
        }
    }
}
