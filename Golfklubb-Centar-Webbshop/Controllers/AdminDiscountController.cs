using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDiscountController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        public AdminDiscountController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Discounts()
        {
            var discounts = await _context.Discounts.ToListAsync(); //Placerar alla Discounts i en lista

            return View(discounts);
        }

        public async Task<IActionResult> DiscountDetails(int id)
        {
            if (id == null) return NotFound();
            var discounts = await _context.Discounts.Include(d => d.Products).FirstOrDefaultAsync(d => d.DiscountId == id);

            if (discounts == null) return NotFound();

            return View(discounts);
        }

        public async Task<IActionResult> DiscountCreate()
        {
            var viewModel = new DiscountCreateViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> DiscountCreate(DiscountCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            _context.Discounts.Add(viewModel.Discount);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rabatten har skapats.";
            return RedirectToAction("Discounts");
        }

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

        [HttpPost]
        public async Task<IActionResult> DiscountEdit(DiscountEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            _context.Discounts.Update(viewModel.Discount);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rabatten har uppdaterats.";
            return RedirectToAction("Discounts", new { id = viewModel.Discount.DiscountId });
        }

        [HttpPost]
        public async Task<IActionResult> DiscountDelete(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rabatten har raderats.";
            return RedirectToAction("Discounts");
        }
    }
}
