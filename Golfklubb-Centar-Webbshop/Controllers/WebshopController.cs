using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using Product = Golfklubb_Centar_Webbshop.Models.Product;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    public class WebshopController : Controller
    {
        private readonly ILogger<WebshopController> _logger;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;
        public WebshopController(ILogger<WebshopController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(int? categoryId)
        {
            var ParentCategories = await _context.Categories
                                         .Where(c => c.FkParentCategoryId == null)
                                         .Include(c => c.InverseFkParentCategory)
                                         .ToListAsync();



            var productsQuery =  _context.Products
                                 .Include(p => p.FkCategory)
                                 .Include(p => p.FkDiscount)
                                 .Include(p => p.ProductReviews)
                                 .AsQueryable();

            
            if (categoryId.HasValue)
            {
                var selected = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
                if (selected != null)
                {
                    if (selected.FkParentCategoryId == null)
                    {
                        var childIds = await _context.Categories
                            .Where(c => c.FkParentCategoryId == categoryId)
                            .Select(c => c.CategoryId)
                            .ToListAsync();
                        productsQuery = productsQuery.Where(p => childIds.Contains(p.FkCategoryId));
                    }
                    else
                    {
                        productsQuery = productsQuery.Where(p => p.FkCategoryId == categoryId);
                    }
                }
            }


            var vm = new CategoryFilterViewModel
            {
                Products = await productsQuery.ToListAsync(),
                ParentCategories = ParentCategories,
                SelectedCategory = categoryId
            };

            return View(vm);
        }

        public async Task<IActionResult> ProductDetails(int? id)
        {

            Product? model = await _context.Products
                                        .Include(p => p.FkCategory)
                                        .Include(p => p.FkDiscount)
                                        .Include(p => p.ProductReviews)
                                        .ThenInclude(u => u.FkUser)
                                        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (model == null) return NotFound();

            var randomProducts = await _context.Products
                                            .Include(p => p.FkCategory)
                                            .Include(p => p.FkDiscount)
                                            .Include(p => p.ProductReviews)
                                            .Where(p => p.ProductId != id)
                                            .OrderBy(p => Guid.NewGuid())
                                            .Take(3)
                                            .ToListAsync();

            var userId = _userManager.GetUserId(User);

            var randomProductsVM = new RandomProductsViewModel
            {
                Product = model,
                RandomProducts = randomProducts,
                HasUserReviewed = model.ProductReviews.Any(r => r.FkUserId == userId)
            };

            return View(randomProductsVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string productReviewContent)
        {

            var userId = _userManager.GetUserId(User);

            if (userId == null) return Challenge();

            bool hasUserReviewed = await _context.ProductReviews
                .AnyAsync(r => r.FkProductId == productId && r.FkUserId == userId);

            if (hasUserReviewed)
                return RedirectToAction(nameof(ProductDetails), new { id = productId });

            var review = new ProductReview
            {
                FkProductId = productId,
                Rating = rating,
                ProductReviewContent = productReviewContent,
                FkUserId = _userManager.GetUserId(User)!
            };
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Recensionen har skickats!";
            return RedirectToAction(nameof(ProductDetails), new {id = productId});
        }

    }
}
