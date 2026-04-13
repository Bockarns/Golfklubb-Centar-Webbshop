using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar notifiktationer för inloggade användare.
    /// Visa, markera som lästa och radera notifikationer.
    /// Kräver inloggad användare.
    /// </summary>
    [Authorize]
    public class NotificationController : Controller
    {    
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        /// <summary>
        /// Visa alla notifikationer för den inloggade användaren. 
        /// Markera olästa som lästa när de visas.
        /// Endast för inloggade användare. Alla kan se sina egna notifikationer, ingen roll krävs.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> NotificationsList()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _context.Notifications
                .Where(n => n.FkUserId == userId)
                .Include(n => n.FkCreatorUser) // Inkludera skaparen av notifikationen
                .Include(n => n.Message)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            // Markera alla olästa notifikationer som lästa

            var unread = notifications.Where(n => !n.IsRead).ToList();
            if (unread.Any())
            {
                foreach (var notification in unread)
                {
                    notification.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return View(notifications);
        }

        ///<summary>
        /// Radera en notifikation.
        /// Endast för inloggade användare. Alla kan radera sina egna notifikationer, ingen roll krävs.
        /// </summary>
        /// <param name="id">ID för notifikationen som ska raderas</param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = _userManager.GetUserId(User);
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.FkUserId == userId);

            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(NotificationsList));
        }
    }
}