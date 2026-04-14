using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Xml.Linq;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Get / Forum
        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _context.Posts
                                        .OrderByDescending(p => p.PostCreateDate)
                                        .Include(p => p.FkUser)
                                        .Include(p => p.Comments)
                                        .ToListAsync();

            return View(posts);
        }

        //Get/ forum/create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.IsForumBanned)
            {
                TempData["Error"] = "Du är blockerad från att använda forumet.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            foreach (var error in ModelState) { Console.WriteLine($"Key: {error.Key}"); foreach (var e in error.Value.Errors) Console.WriteLine($"  Error: {e.ErrorMessage}"); }

            var user = await _userManager.GetUserAsync(User);
            if (user.IsForumBanned)
            {
                TempData["Error"] = "Du är blockerad från att använda forumet.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(post);
            }

            post.FkUserId = user.Id; //Ändrade att använda UserId (Måste vara inloggad för att skapa posts)

            post.PostCreateDate = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Hämta alla användare som följer den som skapade tråden
            var followers = await _context.Follows
                .Where(f => f.FkFollowedUserId == user.Id)
                .ToListAsync();

            // Skapa en notifikation till varje följare

            var creatorUserId = await _userManager.GetUserAsync(User);
            foreach (var follower in followers)
            {
                _context.Notifications.Add(new Notification
                {
                    FkUserId = follower.FkUserId,
                    FkCreatorUser = creatorUserId,
                    Message = $"{user.UserName} skapade en ny tråd: \"{post.PostTitle}\".",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    Link = $"/Forum/Detail/{post.PostId}"
                });
            }

            if (followers.Any())
            {
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id = post.PostId });
        }

        //Get/forum / details / 5

        public async Task<IActionResult> Detail(int id)
        {
            Post? post = await _context.Posts
                                        .Include(p => p.FkUser)
                                        .Include(p => p.Comments
                                        .OrderBy(c => c.CommentDateTime)) //Ändrade så den hämtar comment datetime istället för post datetime
                                        .ThenInclude(c => c.FkUser)
                                        .FirstOrDefaultAsync(p => p.PostId == id); //Byte från ForumPostId till korrekt Id
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        //post/forum/reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Reply(int postId, string content) //Tog bort Username
        {
            var user = await _userManager.GetUserAsync(User);
            if (user.IsForumBanned)
            {
                TempData["Error"] = "Du är blockerad från att använda forumet.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction(nameof(Detail), new { id = postId });
            }
            Post? post = await _context.Posts
                .Include(p => p.FkUser)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            Comment comments = new()
            {
                FkPostId = postId,
                CommentContent = content,
                FkUserId = user.Id,
                CommentDateTime = DateTime.UtcNow
            };

            _context.Comments.Add(comments);

            // Samla alla som ska få notifikation (exkludera den som kommenterar)
            var notifyUserIds = new HashSet<string>();

            // Trådägaren
            if (post.FkUserId != user.Id)
                notifyUserIds.Add(post.FkUserId);

            // Alla som tidigare kommenterat på tråden
            var previousCommenters = post.Comments
                .Select(c => c.FkUserId)
                .Where(id => id != user.Id && id != post.FkUserId)
                .Distinct();

            foreach (var commenterId in previousCommenters)
                notifyUserIds.Add(commenterId);

            // Skapa notifikationer
            foreach (var userId in notifyUserIds)
            {
                var isPostOwner = userId == post.FkUserId;

                _context.Notifications.Add(new Notification
                {
                    FkUserId = userId,
                    FkCreatorUserId = user.Id,
                    Message = isPostOwner
                        ? $"{user.UserName} kommenterade på din tråd: \"{post.PostTitle}\"."
                        : $"{user.UserName} kommenterade på en tråd du deltagit i: \"{post.PostTitle}\".",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    Link = $"/Forum/Detail/{postId}"
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = postId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostDelete(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && post.FkUserId != userId)
                return RedirectToAction(nameof(Index));

            // Notifikation till trådägaren om det är admin som raderar
            if (isAdmin && post.FkUserId != userId)
            {
                _context.Notifications.Add(new Notification
                {
                    FkUserId = post.FkUserId,
                    FkCreatorUserId = userId,
                    Message = $"Din tråd \"{post.PostTitle}\" har raderats av en administratör.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    Link = "/Forum"
                });
            }

            _context.Comments.RemoveRange(post.Comments);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Inlägget har raderats.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.FkPost)
                .FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            if (comment.FkUserId != user.Id && !isAdmin)
                return Forbid();

            // Notifikation till kommentarägaren om det är admin som raderar
            if (isAdmin && comment.FkUserId != user.Id)
            {
                _context.Notifications.Add(new Notification
                {
                    FkUserId = comment.FkUserId,
                    FkCreatorUserId = user.Id,
                    Message = $"Din kommentar i tråden \"{comment.FkPost?.PostTitle}\" har raderats av en administratör.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    Link = $"/Forum/Detail/{comment.FkPostId}"
                });
            }

            int postId = comment.FkPostId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kommentaren har raderats.";
            return RedirectToAction(nameof(Detail), new { id = postId });
        }
    }
}
      

