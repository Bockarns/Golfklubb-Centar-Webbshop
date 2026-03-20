using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                                        .ToListAsync();

            return View(products);
        }
    }
}
