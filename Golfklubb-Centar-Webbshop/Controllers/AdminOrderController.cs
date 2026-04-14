using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar orderadministration i adminpanelen.
    /// Ger admin möjlighet att se och uppdatera inkommande beställningar.
    /// Kräver att användaren är inloggad som Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminOrderController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Visar en lista över alla beställningar inklusive kundinformation.
        /// </summary>
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders.Include(o => o.FkUser).OrderByDescending(o => o.OrderId).ToListAsync();
            return View(orders);
        }

        /// <summary>
        /// Visar detaljer för en specifik order inklusive
        /// orderrader, produkter och faktura.
        /// </summary>
        /// <param name="id">Orderns ID</param>
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders.Include(o => o.FkUser)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FkProduct)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if(order is null)
            {
                return NotFound();
            }

            return View(order);
        }

        /// <summary>
        /// Uppdaterar status på en order.
        /// Möjliga statusar: Pending, Processing, Shipped, Delivered, Cancelled.
        /// </summary>
        /// <param name="id">Orderns ID</param>
        /// <param name="status">Ny orderstatus</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.FkUser)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            order.OrderStatus = status;
            order.StatusDate = DateTime.UtcNow;

            // Notifikation till användaren om statusändring
            _context.Notifications.Add(new Notification
            {
                FkUserId = order.FkUserId,
                FkCreatorUserId = _userManager.GetUserId(User),
                Message = $"Din order #{order.OrderId} har uppdaterats till: {status}.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Link = "/Identity/Account/Manage/OrderHistory"
            });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Orderstatus uppdaterades till " + status + ".";
            return RedirectToAction("OrderDetails", new { id });
        }
    }
}
