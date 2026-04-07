using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminOrderController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders.Include(o => o.FkUser).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var orders = await _context.Orders.Include(o => o.FkUser).Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);
            return View(orders);
        }
    }
}
