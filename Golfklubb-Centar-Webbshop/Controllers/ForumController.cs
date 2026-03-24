using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get / Forum
        public async Task<IActionResult> Index()
        {
            List<ForumPosts> posts = await _context.ForumPosts
                                        .OrderByDescending(p => p.CreatedAt)
                                        .Include(p => p.FkUser)
                                        .ToListAsync();

            return View(posts);
        }

        //Get/ forum/create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ForumPosts post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }
            if (User.Identity?.IsAuthenticated == true)
            {
                post.username = User.Identity.Name ?? "Anonym användare";
            }

            post.CreatedAt = DateTime.UtcNow;
            _context.ForumPosts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = post.ForumPostsId });
        }

        //Get/forum / details / 5

        public async Task<IActionResult> Detail(int id)
        {
            ForumPosts? post = await _context.ForumPosts
                                        .Include(p => p.Replies.OrderBy(r.CreatedAt))
                                        .FirstOrDefaultAsync(p => p.ForumPostsId == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        //post/forum/reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int postId, string content, string? username)
        {
            if (string.isNullOrWhiteSpace(content))
            {
                return RedirectToAction(nameof(Detail), new { id = postId });
            }
            ForumPosts? post = await _context.ForumPosts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            string resolvedUsername = "Anonym användare";
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    resolvedUsername = User.Identity.Name ?? resolvedUsername;
                }
                else if (!string.IsNullOrWhiteSpace(username))
                {
                    resolvedUsername = username.Length > 250 ? username[..250] : username;
                }
                ForumReply reply = new()
                {
                    ForumPostsId = postId,
                    content = content,
                    username = resolvedUsername,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ForumReplies.Add(reply);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Detail), new { id = postId });
            }
        }
    }
}
