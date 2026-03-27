using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminProductController(ILogger<AdminController> logger, ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
        }
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
                if (viewModel.ProductImgPath != null && viewModel.ProductImgPath.Length > 0)
                {
                    var fileName = Path.GetFileName(viewModel.ProductImgPath.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images", "products", fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await viewModel.ProductImgPath.CopyToAsync(stream);

                    viewModel.Product.ProductImgPath = $"/images/products/{fileName}";
                }

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
                if (viewModel.ProductImgPath != null && viewModel.ProductImgPath.Length > 0)
                {
                    //Ta bort gamla bilden 
                    if (!string.IsNullOrEmpty(viewModel.Product.ProductImgPath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, viewModel.Product.ProductImgPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    //Nu vi ersätter bilden som vi gjorde i ProductCreate
                    var fileName = Path.GetFileName(viewModel.ProductImgPath.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images", "products", fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await viewModel.ProductImgPath.CopyToAsync(stream);

                    viewModel.Product.ProductImgPath = $"/images/products/{fileName}";
                }
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
            if (product.Stocks.Any(s => s.Quantity > 0))
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
    }
}
