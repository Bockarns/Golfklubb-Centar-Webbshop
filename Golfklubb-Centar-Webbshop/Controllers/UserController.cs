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

        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ILogger<UserController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

                //Kommentarer
                Comments = await _context.Comments
                    .Include(c => c.FkPost)
                    .Where(c => c.FkUserId == id)
                    .OrderByDescending(c => c.CommentDateTime)
                    .ToListAsync(),

                //Följare och följda
                CommentCount = await _context.Comments
                    .CountAsync(c => c.FkUserId == id),
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
