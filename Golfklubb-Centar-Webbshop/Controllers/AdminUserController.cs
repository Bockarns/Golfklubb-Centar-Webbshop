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
    /// Inkluderar visning, redigering, rollhantering, 
    /// forumblockering och radering av användare.
    /// Kräver att användaren är inloggad som Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly ILogger<AdminUserController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminUserController(ILogger<AdminUserController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Visar en lista över alla registrerade användare.
        /// </summary>
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        /// <summary>
        /// Visar detaljer för en specifik användare inklusive
        /// roller och forumstatus.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                EmailConfirmed = user.EmailConfirmed,
                IsForumBanned = user.IsForumBanned,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return View(viewModel);
        }

        /// <summary>
        /// Visar formulär för att redigera en befintlig användare.
        /// Laddar in alla tillgängliga roller samt användarens nuvarande roller.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        [HttpGet]
        public async Task<IActionResult> UserEdit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                AllRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .Where(r => r != null)
                    .ToListAsync(),
                UserRoles = await _userManager.GetRolesAsync(user)
            };

            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar ändringar för en befintlig användare.
        /// Uppdaterar email, användarnamn, telefonnummer och roller.
        /// Tar bort alla nuvarande roller innan de nya tilldelas.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        /// <param name="email">Nytt email</param>
        /// <param name="userName">Nytt användarnamn</param>
        /// <param name="phoneNumber">Nytt telefonnummer</param>
        /// <param name="selectedRoles">Valda roller från formuläret</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(string id, string email, string userName, string phoneNumber, string[] selectedRoles, string fullname, string profileimageurl)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = email;
            user.FullName = fullname;
            user.ProfileImageUrl = profileimageurl;
            user.UserName = userName;
            user.PhoneNumber = phoneNumber;

            await _userManager.UpdateAsync(user);

            // Ta bort alla nuvarande roller och tilldela de valda rollerna
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, selectedRoles);

            TempData["Success"] = "Användaren " + user.UserName + " uppdaterades.";
            return RedirectToAction("UserDetails", new { id = user.Id });
        }

        /// <summary>
        /// Raderar profilbilden för en användare.
        /// Tar bort bildfilen från servern och nollställer ProfileImageUrl i databasen.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileImage(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Ta bort bildfilen från servern om den finns
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath,
                    user.ProfileImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            user.ProfileImageUrl = null;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profilbilden för " + user.UserName + " har raderats.";
            return RedirectToAction("UserEdit", new { id });
        }

        /// <summary>
        /// Togglar forumblockering för en användare.
        /// En blockerad användare kan inte skapa inlägg eller kommentera i forumet
        /// men kan fortfarande handla i webshopen.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleForumBan(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsForumBanned = !user.IsForumBanned;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.IsForumBanned
                ? "Användaren " + user.UserName + " är nu blockerad från forumet."
                : "Användaren " + user.UserName + " är nu avblockerad från forumet.";

            return RedirectToAction("UserDetails", new { id = user.Id });
        }

        /// <summary>
        /// Raderar en användare permanent från databasen.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                var userName = user.UserName;
                await _userManager.DeleteAsync(user);

                TempData["Success"] = "Användaren " + userName + " har raderats.";
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid radering av användare med ID {UserId}", id);
                TempData["Error"] = "Ett fel inträffade när användaren skulle raderas. Försök igen senare.";
                return RedirectToAction("UserDetails", new { id });
            }
        }

        /// <summary>
        /// Visar orderhistorik för en specifik användare.
        /// Hämtar ordrar med tillhörande orderrader och produkter.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        public async Task<IActionResult> UserOrderHistory(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new UserOrderHistoryViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.FkProduct)
                    .Where(o => o.FkUserId == id)
                    .OrderByDescending(o => o.OrderId)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Visar forumhistorik för en specifik användare.
        /// Hämtar både trådar och kommentarer skapade av användaren.
        /// </summary>
        /// <param name="id">Användarens ID</param>
        public async Task<IActionResult> UserForumHistory(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new UserForumHistoryViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Posts = await _context.Posts
                    .Where(p => p.FkUserId == id)
                    .OrderByDescending(p => p.PostCreateDate)
                    .ToListAsync(),
                Comments = await _context.Comments
                    .Include(c => c.FkPost)
                    .Where(c => c.FkUserId == id)
                    .OrderByDescending(c => c.CommentDateTime)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}