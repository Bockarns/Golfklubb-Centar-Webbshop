using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar CRUD-operationer för kategorier i adminpanelen.
    /// Kräver att användaren är inloggad som Admin.
    /// </summary>
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

        /// <summary>
        /// Visar en lista över alla kategorier inklusive deras föräldrakategori.
        /// </summary>
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.FkParentCategory)
                .ToListAsync();

            return View(categories);
        }

        /// <summary>
        /// Visar detaljer för en specifik kategori, inklusive
        /// föräldrakategori och kopplade produkter.
        /// </summary>
        /// <param name="id">Kategorins ID</param>
        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _context.Categories
                .Include(c => c.FkParentCategory)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            return View(category);
        }

        /// <summary>
        /// Visar formulär för att skapa en ny kategori.
        /// Laddar in alla befintliga kategorier som valbara föräldrakategorier.
        /// </summary>
        public IActionResult CategoryCreate()
        {
            var viewModel = new CategoryCreateViewModel
            {
                ParentCategories = _context.Categories.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar en ny kategori till databasen.
        /// Om validering misslyckas visas formuläret igen med felmeddelanden.
        /// </summary>
        /// <param name="viewModel">Formulärdata för den nya kategorin</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryCreate(CategoryCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(viewModel.Category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategorin " + viewModel.Category.CategoryName + " är skapad.";
                return RedirectToAction("Categories");
            }

            // Fyll i listan igen om validering misslyckas
            viewModel.ParentCategories = _context.Categories.ToList();
            return View(viewModel);
        }

        /// <summary>
        /// Visar formulär för att redigera en befintlig kategori.
        /// Exkluderar kategorin själv från listan över möjliga föräldrakategorier.
        /// </summary>
        /// <param name="id">Kategorins ID</param>
        public async Task<IActionResult> CategoryEdit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var viewModel = new CategoryEditViewModel
            {
                Category = category,
                ParentCategories = _context.Categories
                    .Where(c => c.CategoryId != id) // En kategori kan inte vara sin egen förälder
                    .ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar ändringar för en befintlig kategori.
        /// Om validering misslyckas visas formuläret igen med felmeddelanden.
        /// </summary>
        /// <param name="viewModel">Formulärdata med uppdaterad kategoriinformation</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit(CategoryEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(viewModel.Category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategorin " + viewModel.Category.CategoryName + " är uppdaterad.";
                return RedirectToAction("Categories");
            }

            // Fyll i listan igen om validering misslyckas
            viewModel.ParentCategories = _context.Categories
                .Where(c => c.CategoryId != viewModel.Category.CategoryId)
                .ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Raderar en kategori om den inte har några kopplade produkter eller subkategorier.
        /// Visar felmeddelande om kategorin inte kan raderas.
        /// </summary>
        /// <param name="id">Kategorins ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.InverseFkParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();

            // Kan inte radera om kategorin har kopplade produkter
            if (category.Products.Any())
            {
                TempData["Error"] = "Kan inte radera kategorin, den har produkter kopplade till sig.";
                return RedirectToAction("CategoryDetails", new { id });
            }

            // Kan inte radera om kategorin har subkategorier
            if (category.InverseFkParentCategory.Any())
            {
                TempData["Error"] = "Kan inte radera kategorin, den har subkategorier kopplade till sig.";
                return RedirectToAction("CategoryDetails", new { id });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kategorin " + category.CategoryName + " är raderad.";
            return RedirectToAction("Categories");
        }
    }
}