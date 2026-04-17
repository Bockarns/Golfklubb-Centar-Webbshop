using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar CRUD-operationer för rabatter i adminpanelen.
    /// Kräver att användaren är inloggad som Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminDiscountController : Controller
    {
        private readonly ILogger<AdminDiscountController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminDiscountController(ILogger<AdminDiscountController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Visar en lista över alla rabatter.
        /// </summary>
        public async Task<IActionResult> Discounts()
        {
            var discounts = await _context.Discounts.ToListAsync();
            return View(discounts);
        }

        /// <summary>
        /// Visar detaljer för en specifik rabatt inklusive
        /// produkter kopplade till rabatten.
        /// </summary>
        /// <param name="id">Rabattens ID</param>
        public async Task<IActionResult> DiscountDetails(int id)
        {
            var discount = await _context.Discounts
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d => d.DiscountId == id);

            if (discount == null) return NotFound();

            return View(discount);
        }

        /// <summary>
        /// Visar formulär för att skapa en ny rabatt.
        /// </summary>
        public async Task<IActionResult> DiscountCreate()
        {
            var viewModel = new DiscountCreateViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar en ny rabatt till databasen.
        /// Om validering misslyckas visas formuläret igen med felmeddelanden.
        /// </summary>
        /// <param name="viewModel">Formulärdata för den nya rabatten</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DiscountCreate(DiscountCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            _context.Discounts.Add(viewModel.Discount);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rabatten har skapats.";
            return RedirectToAction("Discounts");
        }

        /// <summary>
        /// Visar formulär för att redigera en befintlig rabatt.
        /// Laddar in produkter kopplade till rabatten.
        /// </summary>
        /// <param name="id">Rabattens ID</param>
        [HttpGet]
        public async Task<IActionResult> DiscountEdit(int id)
        {
            var discount = await _context.Discounts
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d => d.DiscountId == id);

            if (discount == null) return NotFound();

            var viewModel = new DiscountEditViewModel
            {
                Discount = discount,
                Products = discount.Products.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar ändringar för en befintlig rabatt.
        /// Om validering misslyckas visas formuläret igen med felmeddelanden.
        /// </summary>
        /// <param name="viewModel">Formulärdata med uppdaterad rabattinformation</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DiscountEdit(DiscountEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            _context.Discounts.Update(viewModel.Discount);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rabatten har uppdaterats.";
            return RedirectToAction("DiscountDetails", new { id = viewModel.Discount.DiscountId });
        }

        /// <summary>
        /// Raderar en rabatt från databasen.
        /// </summary>
        /// <param name="id">Rabattens ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DiscountDelete(int id)
        {
            try
            {
                var discount = await _context.Discounts.FindAsync(id);
                if (discount == null) return NotFound();

                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Rabatten " + discount.DiscountDescribtion + " är raderad.";
                return RedirectToAction("Discounts");
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "Fel vid radering av rabatt med ID {DiscountId}", id);
                TempData["Error"] = "Ett fel inträffade vid radering av rabatten. Försök igen senare.";
                return RedirectToAction("Discounts");
            }

        }
    }
}