using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar följning av användare i forumet.
    /// Skapa notifiation när en användare följer en annan användare.
    /// Endast för inloggade användare. Alla kan följa varandra, ingen roll krävs.
    /// </summary>
    [Authorize]
    public class FollowController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public FollowController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Toggla följa/avfölja en användare. 
        /// Skapa en notifikation till den följda användaren när någon följer hen.
        /// </summary>
        /// <param name="followedUserId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFollow(string followedUserId)
        {
            var currentUserId = _userManager.GetUserId(User);

            //Kontroll för att unte följa sig själv.
            if (currentUserId == followedUserId)
            {
                //return BadRequest("Du kan inte följa dig själv.");
                TempData["Error"] = "Du kan inte följa dig själv.";
                return RedirectToAction("Index", "Forum");
            }

            var exsistingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FkUserId == currentUserId && f.FkFollowedUserId == followedUserId);

            //Avfölja
            if (exsistingFollow != null)
            {
                _context.Follows.Remove(exsistingFollow);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Du har avföljt användaren.";
                return RedirectToAction("Index", "Forum");

            }
            else
            {
                //Följa
                var newFollow = new Follow
                {
                    FkUserId = currentUserId,
                    FkFollowedUserId = followedUserId,
                    FollowDate = DateTime.UtcNow
                };
                _context.Follows.Add(newFollow);
                // Skapa notifikation
                var currentUser = await _userManager.GetUserAsync(User);
                var creatorUserId = await _userManager.GetUserAsync(User);
                var notification = new Notification
                {
                    FkUserId = followedUserId,
                    FkCreatorUser = creatorUserId,
                    Message = $"Du har en ny följare: {currentUser.UserName}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Du följer nu användaren.";
                return RedirectToAction("Index", "Forum");
            }
        }

        /// <summary>
        /// Visar en lista över användare som följer den inloggade användaren.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Followers()
        {
            var userid = _userManager.GetUserId(User);

            var followers = await _context.Follows
                .Include(f => f.FkUser)
                .Where(f => f.FkFollowedUserId == userid)
                .OrderByDescending(f => f.FollowDate)
                .ToListAsync();

            return View(followers);
        }

        /// <summary>
        /// Visar en lista över användare som den inloggade användaren följer.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Following()
        {
            var userid = _userManager.GetUserId(User);

            var following = await _context.Follows
                .Include(f => f.FkFollowedUser)
                .Where(f => f.FkUserId == userid)
                .OrderByDescending(f => f.FollowDate)
                .ToListAsync();

            return View(following);
        }
    }
}
