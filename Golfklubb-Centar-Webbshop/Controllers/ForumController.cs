using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; //la till usermanager för att lättare kunna hantera användare

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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            foreach (var error in ModelState) { Console.WriteLine($"Key: {error.Key}"); foreach (var e in error.Value.Errors) Console.WriteLine($"  Error: {e.ErrorMessage}"); }

            if (!ModelState.IsValid)
            {
                return View(post);
            }

            post.FkUserId = _userManager.GetUserId(User); //Ändrade att använda UserId (Måste vara inloggad för att skapa posts)

            post.PostCreateDate = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = post.PostId });
        }

        //Get/forum / details / 5

        public async Task<IActionResult> Detail(int id)
        {
            Post? post = await _context.Posts
                                        .Include(p => p.FkUser)
                                        .Include(p => p.Comments
                                        .OrderBy(c => c.CommentDateTime)) //Ändrade så den hämtar comment datetime istället för post datetime
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
            if (string.IsNullOrWhiteSpace(content)) //Bytt till stort I i början på IsNull...
            {
                return RedirectToAction(nameof(Detail), new { id = postId });
            }
            Post? post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            Comment comments = new()
            {
                FkPostId = postId,
                CommentContent = content,
                FkUserId = _userManager.GetUserId(User),
                CommentDateTime = DateTime.UtcNow
            };

            _context.Comments.Add(comments);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = postId });
        }
    }
}

//Snyggt jobbat Anna, vi gjorde några små korrigeringar som tog bort anonym användare, injecerade UserManager för enklare hantering av User och UserId. Samt Löste dom små fel som fanns.
//Grymt gjort. Den är redo för att skapa views och hela den biten :D