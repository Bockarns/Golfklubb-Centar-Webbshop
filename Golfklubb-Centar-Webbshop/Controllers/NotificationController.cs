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
        private readonly ILogger<NotificationController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController(ILogger<NotificationController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
        ///Markera specifik notifikation som läst.
        /// </summary>
        ///<param name="id">Notifiktations ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id && n.FkUserId == userId);
            if (notification == null)
            {
                return NotFound();
            }
            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(NotificationsList));
        }

        ///<summary>
        /// Radera en notifikation.
        /// Endast för inloggade användare. Alla kan radera sina egna notifikationer, ingen roll krävs.
        /// </summary>
        /// <param name="id">ID för notifikationen som ska raderas</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid radering av notifikation med id: {NotificationId}", id);
                TempData["Error"] = "Ett fel inträffade när notifikationen skulle raderas. Försök igen senare.";
                return RedirectToAction(nameof(NotificationsList));
            }
        }
        /// <summary>
        /// Returnera alla olästa notifikationer för den inloggade användaren som en Json
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<IActionResult> UnreadCounter()
        {
            var userId = _userManager.GetUserId(User);

            var count = await _context.Notifications.CountAsync(n => n.FkUserId == userId && !n.IsRead);

            return Json(count);
        }
    }
}