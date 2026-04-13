using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hantera publika användarprofiler. Visa användarens information, inlägg och följare/följda.
    /// </summary>
    public class UserController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Visare publik profil för specifik användare. 
        /// Inkluderar användarens inlägg, antal följare och följda, 
        /// samt om den aktuella användaren följer eller är följare av profilen.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profileUser = await _userManager.FindByIdAsync(id);

            if (profileUser == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new UserProfileViewModel
            {
                UserId = profileUser.Id,
                UserName = profileUser.UserName,
                ProfileImageUrl = profileUser.ProfileImageUrl,
                IsCurrentUser = currentUserId == profileUser.Id,

                //Foruminlägg
                Posts = await _context.Posts
                    .Where(p => p.FkUserId == id)
                    .OrderByDescending(p => p.PostCreateDate)
                    .ToListAsync(),

                //Följare och följda
                FollowersCount = await _context.Follows
                    .CountAsync(f => f.FkFollowedUserId == id),
                FollowingCount = await _context.Follows
                    .CountAsync(f => f.FkUserId == id),

                //Kolla om den aktuella användaren följer eller är följare av profilen
                IsFollowing = currentUserId != null && await _context.Follows
                    .AnyAsync(f => f.FkUserId == currentUserId && f.FkFollowedUserId == id),
                IsFollower = currentUserId != null && await _context.Follows
                    .AnyAsync(f => f.FkUserId == id && f.FkFollowedUserId == currentUserId)
            };




            return View(viewModel);
        }
    }
}
