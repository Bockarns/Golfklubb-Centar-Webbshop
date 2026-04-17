using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class KvittoModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public KvittoModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Order? Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            Order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FkProduct)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.FkUserId == user.Id);

            if (Order == null)
            {
                return NotFound();
            }


            return Page();
        }
    }
}