using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Controllers
{
    /// <summary>
    /// Hanterar CRUD-operationer för produkter i adminpanelen.
    /// Kräver att användaren är inloggad som Admin.
    /// </summary>
    [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Visar en lista över alla produkter.
        /// </summary>
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        /// <summary>
        /// Visar detaljer för en specifik produkt inklusive
        /// kategori, rabatt och lagersaldo.
        /// </summary>
        /// <param name="id">Produktens ID</param>
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.FkCategory)
                .Include(p => p.FkDiscount)
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductDescribtion = product.ProductDescribtion,
                ProductImgPath = product.ProductImgPath,
                CategoryName = product.FkCategory?.CategoryName,
                DiscountDescription = product.FkDiscount?.DiscountDescribtion,
                Stocks = product.Stocks.Sum(s => s.Quantity)
            };

            return View(viewModel);
        }

        /// <summary>
        /// Visar formulär för att skapa en ny produkt.
        /// Laddar in kategorier och rabatter för dropdowns.
        /// Sätter standardrabatt till ID 1 (Ingen rabatt).
        /// </summary>
        [HttpGet]
        public IActionResult ProductCreate()
        {
            var viewModel = new ProductCreateViewModel
            {
                Product = new Product
                {
                    FkDiscountId = 1 // Standardvärde: Ingen rabatt
                },
                // Hämta endast de kategorier som är barn (där FkParentCategoryId INTE är null)
                Categories = _context.Categories
                    .Where(c => c.FkParentCategoryId != null)
                    .ToList(),
                Discounts = _context.Discounts.ToList()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar en ny produkt till databasen.
        /// Hanterar även uppladdning av produktbild och skapande av lagerpost.
        /// Två SaveChanges behövs, först för att få ett ProductId,
        /// sedan för att koppla Stock till produkten.
        /// Om validering misslyckas visas formuläret igen med felmeddelanden.
        /// ska lägga till en kontroll om kategori har valts. annars visas formuläret igen med felmeddelanden eller så skapar vi en ny kategori "Ny produkt" tex 
        /// som vi lägger som standard som ovan med discount.
        /// </summary>
        /// <param name="viewModel">Formulärdata för den nya produkten</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductCreateViewModel viewModel)
        {
            if (viewModel.Product.FkCategoryId != 0)
            {
                // Hämta den valda kategorin från databasen
                var selectedCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryId == viewModel.Product.FkCategoryId);

                // Kontrollera om kategorin saknar Parent (vilket gör den till en huvudkategori)
                if (selectedCategory != null && selectedCategory.FkParentCategoryId == null)
                {
                    ModelState.AddModelError("Product.FkCategoryId",
                        "Produkter kan endast kopplas till underkategorier. Vänligen välj eller skapa en underkategori först.");
                }
            }
            else
            {
                ModelState.AddModelError("Product.FkCategoryId", "Du måste välja en kategori.");
            }
            if (ModelState.IsValid)
            {
                // Hantera bilduppladdning om en bild skickats med
                if (viewModel.ProductImgPath != null && viewModel.ProductImgPath.Length > 0)
                {
                    var fileName = Path.GetFileName(viewModel.ProductImgPath.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images", "products", fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await viewModel.ProductImgPath.CopyToAsync(stream);

                    viewModel.Product.ProductImgPath = $"/images/products/{fileName}";
                }

                // Spara produkten först för att få ett ProductId
                _context.Products.Add(viewModel.Product);
                await _context.SaveChangesAsync();

                // Skapa lagerpost kopplad till den nya produkten
                _context.Stocks.Add(new Stock
                {
                    FkProductId = viewModel.Product.ProductId,
                    Quantity = viewModel.StockQuantity
                });

                await _context.SaveChangesAsync();
                TempData["Success"] = "Produkten " + viewModel.Product.ProductName + " är skapad.";
                return RedirectToAction("Products");
            }

            // Vid fel: Ladda om listorna men filtrera gärna så bara underkategorier visas i dropdownen
            viewModel.Categories = _context.Categories
                .Where(c => c.FkParentCategoryId != null)
                .ToList();
            viewModel.Discounts = _context.Discounts.ToList();
            return View(viewModel);
        }

        /// <summary>
        /// Visar formulär för att redigera en befintlig produkt.
        /// Laddar in kategorier, rabatter och nuvarande lagersaldo.
        /// </summary>
        /// <param name="id">Produktens ID</param>
        [HttpGet]
        public async Task<IActionResult> ProductEdit(int id)
        {
            var product = await _context.Products
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            var viewModel = new ProductEditViewModel
            {
                Product = product,
                // Endast underkategorier (child) ska vara valbara
                Categories = _context.Categories
                    .Where(c => c.FkParentCategoryId != null)
                    .ToList(),
                Discounts = _context.Discounts.ToList(),
                StockQuantity = product.Stocks.Sum(s => s.Quantity),
                ExistingImgPath = product.ProductImgPath // Se till att vi sparar befintlig sökväg
            };

            return View(viewModel);
        }

        /// <summary>
        /// Tar emot och sparar ändringar för en befintlig produkt.
        /// Hanterar byte av produktbild, tar bort gammal bild innan ny sparas.
        /// Uppdaterar befintlig lagerpost eller skapar en ny om ingen finns.
        /// Om validering misslyckas visas formuläret igen med felmeddelanden.
        /// </summary>
        /// <param name="viewModel">Formulärdata med uppdaterad produktinformation</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductEditViewModel viewModel)
        {
            // Säkerhetskontroll: Är den valda kategorin en underkategori?
            if (viewModel.Product.FkCategoryId != 0)
            {
                var selectedCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryId == viewModel.Product.FkCategoryId);

                if (selectedCategory != null && selectedCategory.FkParentCategoryId == null)
                {
                    ModelState.AddModelError("Product.FkCategoryId",
                        "Produkter måste tillhöra en underkategori. Vänligen välj en annan kategori.");
                }
            }

            if (ModelState.IsValid)
            {
                // --- Din befintliga logik för bildhantering ---
                if (viewModel.ProductImgPath != null && viewModel.ProductImgPath.Length > 0)
                {
                    if (!string.IsNullOrEmpty(viewModel.ExistingImgPath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, viewModel.ExistingImgPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    var fileName = Path.GetFileName(viewModel.ProductImgPath.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images", "products", fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await viewModel.ProductImgPath.CopyToAsync(stream);

                    viewModel.Product.ProductImgPath = $"/images/products/{fileName}";
                }
                else
                {
                    viewModel.Product.ProductImgPath = viewModel.ExistingImgPath;
                }

                _context.Products.Update(viewModel.Product);

                // --- Lagerhantering ---
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
                TempData["Success"] = $"Produkten {viewModel.Product.ProductName} är uppdaterad.";
                return RedirectToAction("ProductDetails", new { id = viewModel.Product.ProductId });
            }

            // Fyll i listorna igen om validering misslyckas
            viewModel.Categories = _context.Categories
                .Where(c => c.FkParentCategoryId != null)
                .ToList();
            viewModel.Discounts = _context.Discounts.ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Raderar en produkt från databasen tillsammans med dess lagerpost.
        /// Kan inte radera om produkten finns i en varukorg, faktura eller har varor i lager.
        /// </summary>
        /// <param name="id">Produktens ID</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(int id)
        {
            var product = await _context.Products
                .Include(p => p.InvoiceItems)
                .Include(p => p.ProductReviews)
                .Include(p => p.Stocks)
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // Kan inte radera om produkten finns i en aktiv varukorg

            //if (product.CartItems.Any())
            //{
            //    TempData["Error"] = "Kan inte radera produkten, den finns i en eller flera varukorgar.";
            //    return RedirectToAction("ProductDetails", new { id });
            //}

            // Kan inte radera om produkten finns i en faktura eller order
            if (product.InvoiceItems.Any())
            {
                TempData["Error"] = "Kan inte radera produkten, den finns i en eller flera fakturor.";
                return RedirectToAction("ProductDetails", new { id });
            }

            if (product.OrderItems.Any())
            {
                TempData["Error"] = "Kan inte radera produkten, den finns i en eller flera beställningar.";
                return RedirectToAction("ProductDetails", new { id });
            }

            // Kan inte radera om det finns varor i lager
            if (product.Stocks.Any(s => s.Quantity > 0))
            {
                TempData["Error"] = "Kan inte radera produkten, det finns varor i lagret.";
                return RedirectToAction("ProductDetails", new { id });
            }

            // Ta bort lagerpost och produkt

            //Denna rad är om man behöver ta bort en produkt som finns i en order, tillfälligt under produktion.
            //Någon som hade varit bra vid ett riktigt är att man haft en tabell separat för produkter som ska utgå från lagret men fortfarande synas i orderhistoriken.
            //_context.OrderItems.RemoveRange(product.OrderItems);

            _context.Stocks.RemoveRange(product.Stocks);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Produkten " + product.ProductName + " är raderad.";
            return RedirectToAction("Products");
        }
    }
}