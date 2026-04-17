using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar användaradministration i adminpanelen.
    /// Visar Dashboard och Webshop dashboard
    /// Kräver att användaren är inloggad som Admin.
    /// Visar statistik över ordrar, forumaktivitet och recensioner, samt senaste ordrar och trådar.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        /// <summary>
        /// Dashboard-sidan för adminpanelen. 
        /// Visar statistik över ordrar, forumaktivitet och recensioner, 
        /// samt senaste ordrar och trådar.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);

            var viewModel = new AdminDashboardViewModel
            {
                // Orderstatus-counters
                OrdersPending = await _context.Orders
                    .CountAsync(o => o.OrderStatus == "Ny Order"),
                OrdersProcessing = await _context.Orders
                    .CountAsync(o => o.OrderStatus == "Packas"),
                OrdersShipped = await _context.Orders
                    .CountAsync(o => o.OrderStatus == "Skickad"),
                OrdersDelivered = await _context.Orders
                    .CountAsync(o => o.OrderStatus == "Levererad"),
                OrdersCancelled = await _context.Orders
                    .CountAsync(o => o.OrderStatus == "Avbruten"),

                // Forum-counters (senaste 30 dagarna)
                NewPosts = await _context.Posts
                    .CountAsync(p => p.PostCreateDate >= last30Days),
                NewComments = await _context.Comments
                    .CountAsync(c => c.CommentDateTime >= last30Days),

                // Review-counter (senaste 30 dagarna)
                NewReviews = await _context.ProductReviews
                    .CountAsync(r => r.CreatedAt >= last30Days),

                // Senaste 5 ordrar
                LatestOrders = await _context.Orders
                    .Include(o => o.FkUser)
                    .OrderByDescending(o => o.OrderId)
                    .Take(5)
                    .ToListAsync(),

                // Senaste 5 trådar
                LatestPosts = await _context.Posts
                    .Include(p => p.FkUser)
                    .OrderByDescending(p => p.PostCreateDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
        //Webshop page
        public async Task<IActionResult> Webshop()
        {
            return View();
        }
    }
}