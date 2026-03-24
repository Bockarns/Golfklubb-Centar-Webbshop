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
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                                        .Include(p => p.FkCategory)
                                        .Include(p => p.FkDiscount)
                                        .Include(p => p.ProductReviews)
                                        .ToListAsync();

            return View(products);
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
                                            .Where(p => p.ProductId != id)
                                            .OrderBy(p => Guid.NewGuid())
                                            .Take(3)
                                            .ToListAsync();

            var randomProductsVM = new RandomProductsViewModel
            {
                Product = model,
                RandomProducts = randomProducts
            };

            return View(randomProductsVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int productId, int rating, string productReviewContent)
        {
            var review = new ProductReview
            {
                FkProductId = productId,
                Rating = rating,
                ProductReviewContent = productReviewContent,
                FkUserId = _userManager.GetUserId(User)!
            };
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ProductDetails), new {id = productId});
        }
    }
}
