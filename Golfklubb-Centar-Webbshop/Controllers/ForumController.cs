using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Golfklubb_Centar_Webbshop.Models;

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
            List<Post> posts = await _context.Posts
                                        .OrderByDescending(p => p.PostCreateDate)
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

        public async Task<IActionResult> Create(Post post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }
            if (User.Identity?.IsAuthenticated == true)
            {
                post.FkUserId = User.Identity.Name ?? "Anonym användare";
            }

            post.PostCreateDate = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = post.PostId });
        }

        //Get/forum / details / 5

        public async Task<IActionResult> Detail(int id)
        {
            Post? post = await _context.Posts
                                        .Include(p => p.Comments.OrderBy(p.PostCreateDateTime))
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
            Post? post = await _context.Posts.FindAsync(postId);
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
                Comment comments = new()
                {
                    CommentId = postId,
                    CommentContent = content,
                    FkUser = resolvedUsername,
                    CommentDateTime = DateTime.UtcNow
                };

                _context.Comments.Add(comments);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Detail), new { id = postId });
            }
        }
    }
}
