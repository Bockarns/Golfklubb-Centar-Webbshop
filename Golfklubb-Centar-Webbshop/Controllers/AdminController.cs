using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AdminController(ILogger<AdminController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        //Users
        #region
        
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                IsForumBanned = user.IsForumBanned,
                Roles = await _userManager.GetRolesAsync(user)
            };

            return View(viewModel);
        }
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
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                AllRoles = await _roleManager.Roles.Select(r => r.Name!).Where(r => r != null).ToListAsync()!,
                UserRoles = await _userManager.GetRolesAsync(user)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(string id, string email, string userName, string phoneNumber, string[] selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = email;
            user.UserName = userName;
            user.PhoneNumber = phoneNumber;

            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRolesAsync(user, selectedRoles);

            TempData["Success"] = "Användaren uppdaterades.";
            return RedirectToAction("UserDetails", new { id = user.Id });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleForumBan(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsForumBanned = !user.IsForumBanned;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.IsForumBanned? "Användaren är nu blockerad från forumet.": "Användaren är nu avblockerad från forumet.";

            return RedirectToAction("UserDetails", new { id = user.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            TempData["Success"] = "Användaren har raderats.";
            return RedirectToAction("Users");
        }
        #endregion

        //Webshop
        #region
        public async Task<IActionResult> Webshop()
        {
            return View();
        }

        //Kategorier
        #region
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.Include(c => c.FkParentCategory).ToListAsync(); //Placerar alla kategorier i en lista

            return View(categories);
        }

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _context.Categories
                .Include(c => c.FkParentCategory)
                .Include(c => c.Products)  // Hämtar produkter kopplade till kategorin
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        public IActionResult CategoryCreate()
        {
            var viewModel = new CategoryCreateViewModel
            {
                ParentCategories = _context.Categories.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryCreate(CategoryCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(viewModel.Category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            // Fyll igen om validering failar
            viewModel.ParentCategories = _context.Categories.ToList();
            TempData["Success"] = "Kategorin " + viewModel.Category.CategoryName + " är skapad.";
            return View(viewModel);
        }

        public async Task<IActionResult> CategoryEdit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var viewModel = new CategoryEditViewModel
            {
                Category = category,
                ParentCategories = _context.Categories
                    .Where(c => c.CategoryId != id)  // Kan inte vara sin egen förälder
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryEdit(CategoryEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(viewModel.Category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Categories");
            }

            viewModel.ParentCategories = _context.Categories
                .Where(c => c.CategoryId != viewModel.Category.CategoryId)
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.InverseFkParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category.Products.Any())
            {
                TempData["Error"] = "Kan inte radera kategorin, den har produkter kopplade till sig.";
                return RedirectToAction("CategoryDetails", new { id });  // skicka tillbaka till detaljsidan
            }

            if (category.InverseFkParentCategory.Any())
            {
                TempData["Error"] = "Kan inte radera kategorin, den har subkategorier kopplade till sig.";
                return RedirectToAction("CategoryDetails", new { id });
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kategorin är raderad.";
            return RedirectToAction("Categories");
        }
        #endregion
        //Produkter
        #region
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.ToListAsync(); //Placerar alla kategorier i en lista

            return View(products);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.Include(p => p.FkCategory).Include(p => p.FkDiscount).Include(p => p.Stocks).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductDescribtion = product.ProductDescribtion,
                ProductImgPath = product.ProductImgPath,

                FkCategoryId = product.FkCategoryId,
                FkDiscountId = product.FkDiscountId,

                CategoryName = product.FkCategory?.CategoryName,
                DiscountDescription = product.FkDiscount?.DiscountDescribtion,
                Stocks = product.Stocks.Sum(s => s.Quantity)
            };

            return View(viewModel);
        }
        public IActionResult ProductCreate()
        {
            var viewModel = new ProductCreateViewModel
            {
                Product = new Product
                {
                    FkDiscountId = 1 //"Ingen rabatt"
                },
                Categories = _context.Categories.ToList(),
                Discounts = _context.Discounts.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(viewModel.Product);
                await _context.SaveChangesAsync(); //Behövde ha dubbla SaveChanges för att först få ett Id till produkten

                _context.Stocks.Add(new Stock
                {
                    FkProductId = viewModel.Product.ProductId,
                    Quantity = viewModel.StockQuantity
                });

                await _context.SaveChangesAsync();
                TempData["Success"] = "Produkten " + viewModel.Product.ProductName + " är skapad.";
                return RedirectToAction("Products");
            }

            viewModel.Categories = _context.Categories.ToList();
            viewModel.Discounts = _context.Discounts.ToList();
            return View(viewModel);
        }

        public async Task<IActionResult> ProductEdit(int id)
        {
            var product = await _context.Products.Include(p => p.Stocks).FirstOrDefaultAsync(p => p.ProductId == id); ;
            if (product == null) return NotFound();

            var viewModel = new ProductEditViewModel
            {
                Product = product,
                Categories = _context.Categories.ToList(),
                Discounts = _context.Discounts.ToList(),
                StockQuantity = product.Stocks.Sum(s => s.Quantity)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(viewModel.Product);

                // Hämta befintlig stock eller skapa ny
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.FkProductId == viewModel.Product.ProductId);

                if (stock != null)
                {
                    stock.Quantity = viewModel.StockQuantity;
                    _context.Stocks.Update(stock);
                }
                else
                {
                    _context.Stocks.Add(new Stock
                    {
                        FkProductId = viewModel.Product.ProductId,
                        Quantity = viewModel.StockQuantity
                    });
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Produkten " + viewModel.Product.ProductName + " är uppdaterad.";
                return RedirectToAction("Products");
            }

            viewModel.Categories = _context.Categories.ToList();
            viewModel.Discounts = _context.Discounts.ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(int id)
        {
            var product = await _context.Products
                .Include(p => p.CartItems)
                .Include(p => p.InvoiceItems)
                .Include(p => p.ProductReviews)
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            if (product.CartItems.Any())
            {
                TempData["Error"] = "Kan inte radera produkten, den finns i en eller flera varukorgar.";
                return RedirectToAction("ProductDetails", new { id });
            }

            if (product.InvoiceItems.Any())
            {
                TempData["Error"] = "Kan inte radera produkten, den finns i en eller flera fakturor.";
                return RedirectToAction("ProductDetails", new { id });
            }
            if (product.Stocks.Any(s => s.Quantity >0))
            {
                TempData["Error"] = "Kan inte radera produkten, den finns varor i lagret.";
                return RedirectToAction("ProductDetails", new { id });
            }
            _context.Stocks.RemoveRange(product.Stocks);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Produkten " + product.ProductName + " är raderad.";
            return RedirectToAction("Products");
        }
        #endregion
        //Rabatter
        #region
        #endregion
        #endregion
    }
}
