using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private const string CartSessionKey = "GolfklubbCart";
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private List<CartItem> GetCart()
        {
            string? json = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            List<CartItem> cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int productId, string productName, decimal unitPrice, int quantity)
        {
            if (quantity < 1) quantity = 1;

            List<CartItem> cart = GetCart();
            CartItem? existing = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existing is not null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            TempData["CartMessage"] = $"{quantity} × \"{productName}\" tillagd i varukorgen.";
            return RedirectToAction("ProductDetails", "Webshop", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId)
        {
            List<CartItem> cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Increase(int productId)
        {
            List<CartItem> cart = GetCart();
            CartItem? item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item is not null)
                item.Quantity++;
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrease(int productId)
        {
            List<CartItem> cart = GetCart();
            CartItem? item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item is not null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            List<CartItem> cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            ViewData["CartItems"] = cart;
            ViewData["CartTotal"] = cart.Sum(c => c.LineTotal);
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            List<CartItem> cart = GetCart();

            if (!ModelState.IsValid)
            {
                ViewData["CartItems"] = cart;
                ViewData["CartTotal"] = cart.Sum(c => c.LineTotal);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                FkUserId = user!.Id,
                OrderStatus = "Pending",
                FullName = model.FullName,
                Address = model.Address,
                PostalCode = model.PostalCode,
                City = model.City,
                Country = model.Country,
                Phone = model.Phone,
                OrderItems = cart.Select(c => new OrderItem
                {
                    FkProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.UnitPrice,
                    SubTotal = c.LineTotal
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            SaveCart(new List<CartItem>());
            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.FkProduct)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
