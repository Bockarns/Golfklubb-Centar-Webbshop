using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class OrderHistoryModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public OrderHistoryModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<Order> Orders { get; set; } = new List<Order>();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                Orders = new List<Order>();
                return;
            }

            Orders = await _context.Orders
                .Where(o => o.FkUserId == user.Id)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}