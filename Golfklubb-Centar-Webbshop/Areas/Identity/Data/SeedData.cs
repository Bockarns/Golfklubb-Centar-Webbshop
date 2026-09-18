using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Identity;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Data
{
    public class SeedData
    {
        public static async Task SeedAll(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRoles(serviceProvider);

            // Kör i rätt ordning pga foreign keys
            if (!context.Taxes.Any()) await SeedTax(context);
            if (!context.Discounts.Any()) await SeedDiscount(context);
            if (!context.Categories.Any()) await SeedCategory(context);
            if (!context.Products.Any()) await SeedProduct(context);
            if (!context.Stocks.Any()) await SeedStock(context);

            // Användare seedas sist eftersom forum/orders behöver userId
            await SeedUsers(userManager);

            if (!context.Posts.Any()) await SeedForum(context, userManager);
            if (!context.ProductReviews.Any()) await SeedReviews(context, userManager);
        }

        /// <summary>
        /// Skapar rollerna Admin och User samt ett standardadminkonto.
        /// Admin: admin@test.se / Test123!
        /// </summary>
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Admin-konto
            var adminEmail = "admin@test.se";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Admin Adminsson",
                    PhoneNumber = "070-000 00 00"
                };
                await userManager.CreateAsync(admin, "Test123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        /// <summary>
        /// Skapar testanvändare med rollen User.
        /// Lösenord för alla: Test123!
        /// </summary>
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var users = new[]
            {
                new { Email = "anna@test.se",    UserName = "Anna",    FullName = "Anna Andersson",  Phone = "070-111 11 11" },
                new { Email = "bjorn@test.se",   UserName = "Bjorn",   FullName = "Björn Björkman",  Phone = "070-222 22 22" },
                new { Email = "cecilia@test.se", UserName = "Cecilia", FullName = "Cecilia Carlsson", Phone = "070-333 33 33" },
                new { Email = "david@test.se",   UserName = "David",   FullName = "David Davidsson", Phone = "070-444 44 44" },
            };

            foreach (var u in users)
            {
                if (await userManager.FindByEmailAsync(u.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        EmailConfirmed = true,
                        FullName = u.FullName,
                        PhoneNumber = u.Phone
                    };
                    await userManager.CreateAsync(user, "Test123!");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }

        /// <summary>
        /// Skapar en standard-moms för fakturor.
        /// </summary>
        public static async Task SeedTax(ApplicationDbContext context)
        {
            context.Taxes.Add(new Taxis
            {
                TaxIndex = 25,
                TaxDescribtion = "Standard moms 25%"
            });
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar grundläggande rabatter.
        /// </summary>
        public static async Task SeedDiscount(ApplicationDbContext context)
        {
            var discounts = new List<Discount>
            {
                new Discount
                {
                    Discount1 = 0,
                    DiscountDescribtion = "Ordinarie Pris",
                    DiscountType = "%"
                },
                new Discount
                {
                    Discount1 = 10,
                    DiscountDescribtion = "StartRea 10%",
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
                    DiscountType = "%"
                },
                new Discount
                {
                    Discount1 = 20,
                    DiscountDescribtion = "Sommarrea 20%",
                    StartDate = DateOnly.FromDateTime(DateTime.Now),
                    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(2)),
                    DiscountType = "%"
                }
            };
            context.Discounts.AddRange(discounts);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar kategorier med underkategorier.
        /// </summary>
        public static async Task SeedCategory(ApplicationDbContext context)
        {
            // Föräldrakategorier
            var klubbor = new Category { CategoryName = "Klubbor" };
            var klader = new Category { CategoryName = "Kläder" };
            var tillbehor = new Category { CategoryName = "Tillbehör" };
            var skor = new Category { CategoryName = "Skor" };

            context.Categories.AddRange(klubbor, klader, tillbehor, skor);
            await context.SaveChangesAsync();

            // Underkategorier
            var underkategorier = new List<Category>
            {
                new Category { CategoryName = "Drivers",        FkParentCategoryId = klubbor.CategoryId },
                new Category { CategoryName = "Järnklubbor",    FkParentCategoryId = klubbor.CategoryId },
                new Category { CategoryName = "Putters",        FkParentCategoryId = klubbor.CategoryId },
                new Category { CategoryName = "Tröjor",         FkParentCategoryId = klader.CategoryId },
                new Category { CategoryName = "Byxor",          FkParentCategoryId = klader.CategoryId },
                new Category { CategoryName = "Kepsar",         FkParentCategoryId = klader.CategoryId },
                new Category { CategoryName = "Golfbagar",      FkParentCategoryId = tillbehor.CategoryId },
                new Category { CategoryName = "Golfbollar",     FkParentCategoryId = tillbehor.CategoryId },
                new Category { CategoryName = "Handskar",       FkParentCategoryId = tillbehor.CategoryId },
                new Category { CategoryName = "Golfskor",       FkParentCategoryId = skor.CategoryId },
                new Category { CategoryName = "Träningsskor",   FkParentCategoryId = skor.CategoryId },
            };

            context.Categories.AddRange(underkategorier);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar exempelprodukter kopplade till kategorier och rabatter.
        /// </summary>
        public static async Task SeedProduct(ApplicationDbContext context)
        {
            // Hämta kategori-ID:n dynamiskt
            var drivers = context.Categories.First(c => c.CategoryName == "Drivers").CategoryId;
            var jarnklubbor = context.Categories.First(c => c.CategoryName == "Järnklubbor").CategoryId;
            var putters = context.Categories.First(c => c.CategoryName == "Putters").CategoryId;
            var trojor = context.Categories.First(c => c.CategoryName == "Tröjor").CategoryId;
            var byxor = context.Categories.First(c => c.CategoryName == "Byxor").CategoryId;
            var kepsar = context.Categories.First(c => c.CategoryName == "Kepsar").CategoryId;
            var golfbagar = context.Categories.First(c => c.CategoryName == "Golfbagar").CategoryId;
            var golfbollar = context.Categories.First(c => c.CategoryName == "Golfbollar").CategoryId;
            var handskar = context.Categories.First(c => c.CategoryName == "Handskar").CategoryId;
            var golfskor = context.Categories.First(c => c.CategoryName == "Golfskor").CategoryId;

            var ordinarie = context.Discounts.First(d => d.Discount1 == 0).DiscountId;
            var rea10 = context.Discounts.First(d => d.Discount1 == 10).DiscountId;
            var rea20 = context.Discounts.First(d => d.Discount1 == 20).DiscountId;

            var products = new List<Product>
            {
                // Klubbor
                new Product { ProductName = "TaylorMade Stealth Driver",   ProductPrice = 4999.00M, ProductDescribtion = "Avancerad driver med kolfiberkrona för maximal distans.",       FkCategoryId = drivers,     FkDiscountId = ordinarie },
                new Product { ProductName = "Callaway Rogue Iron Set",      ProductPrice = 7999.00M, ProductDescribtion = "Komplett järnset med 5-PW, perfekt för mellannivå.",            FkCategoryId = jarnklubbor, FkDiscountId = rea10 },
                new Product { ProductName = "Titleist Scotty Cameron Putter", ProductPrice = 3499.00M, ProductDescribtion = "Exklusiv putter för precision på greenen.",                  FkCategoryId = putters,     FkDiscountId = ordinarie },

                // Kläder
                new Product { ProductName = "Nike Dri-FIT Golftröja Blå",  ProductPrice = 599.00M,  ProductDescribtion = "Andningsbar tröja med Dri-FIT-teknologi, perfekt för varma dagar.", FkCategoryId = trojor,   FkDiscountId = ordinarie },
                new Product { ProductName = "Adidas Golf Byxor Grå",       ProductPrice = 799.00M,  ProductDescribtion = "Stretch-byxor med 4-vägs stretch för full rörelsefrihet.",       FkCategoryId = byxor,       FkDiscountId = rea10 },
                new Product { ProductName = "Under Armour Keps Svart",     ProductPrice = 349.00M,  ProductDescribtion = "Justerbar keps med UV-skydd och svettabsorberande band.",        FkCategoryId = kepsar,      FkDiscountId = ordinarie },

                // Tillbehör
                new Product { ProductName = "Titleist Tour Staff Bag",     ProductPrice = 5999.00M, ProductDescribtion = "Premium tour-bag med 10 fickor och stabil ställning.",          FkCategoryId = golfbagar,   FkDiscountId = ordinarie },
                new Product { ProductName = "Callaway Supersoft Bollar",   ProductPrice = 299.00M,  ProductDescribtion = "12-pack med låg kompression för ökad distans och mjuk känsla.", FkCategoryId = golfbollar,  FkDiscountId = rea20 },
                new Product { ProductName = "FootJoy RainGrip Handske",    ProductPrice = 199.00M,  ProductDescribtion = "Vattentålig handske som ger bättre grepp i regnigt väder.",     FkCategoryId = handskar,    FkDiscountId = ordinarie },

                // Skor
                new Product { ProductName = "FootJoy Pro SL Golfskor Vit", ProductPrice = 1799.00M, ProductDescribtion = "Vattentäta golfskor med spikfritt grepp och spetsig komfort.",  FkCategoryId = golfskor,    FkDiscountId = ordinarie },
                new Product { ProductName = "Adidas CodeChaos Skor Grön",  ProductPrice = 1499.00M, ProductDescribtion = "Lätta och flexibla golfskor med BOOST-stötdämpning.",           FkCategoryId = golfskor,    FkDiscountId = rea10 },
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar lager för alla produkter.
        /// </summary>
        public static async Task SeedStock(ApplicationDbContext context)
        {
            var products = context.Products.ToList();
            var random = new Random();

            var stocks = products.Select(p => new Stock
            {
                FkProductId = p.ProductId,
                Quantity = random.Next(5, 50)
            }).ToList();

            context.Stocks.AddRange(stocks);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar forumtrådar och kommentarer med testanvändare.
        /// </summary>
        public static async Task SeedForum(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var anna = await userManager.FindByEmailAsync("anna@test.se");
            var bjorn = await userManager.FindByEmailAsync("bjorn@test.se");
            var cecilia = await userManager.FindByEmailAsync("cecilia@test.se");
            var david = await userManager.FindByEmailAsync("david@test.se");

            if (anna == null || bjorn == null) return;

            var posts = new List<Post>
            {
                new Post
                {
                    PostTitle = "Tips för nybörjare på golfbanan?",
                    PostContent = "Hej alla! Jag är ny på golf och undrar om ni har några bra tips för att komma igång. Vad är de viktigaste sakerna att tänka på? 🏌️",
                    FkUserId = anna.Id,
                    PostCreateDate = DateTime.UtcNow.AddDays(-10),
                    Comments = new List<Comment>
                    {
                        new Comment { CommentContent = "Börja med att ta några lektioner, det gör stor skillnad! 👍", FkUserId = bjorn.Id,   CommentDateTime = DateTime.UtcNow.AddDays(-9) },
                        new Comment { CommentContent = "Fokusera på swing-tekniken från början, det är svårare att ändra senare.", FkUserId = cecilia.Id, CommentDateTime = DateTime.UtcNow.AddDays(-8) },
                        new Comment { CommentContent = "Kom ihåg att ha kul! Golf ska vara roligt 😊", FkUserId = david.Id,   CommentDateTime = DateTime.UtcNow.AddDays(-7) },
                    }
                },
                new Post
                {
                    PostTitle = "Bästa golfklubban för mellannivå?",
                    PostContent = "Jag har spelat golf i 2 år och funderar på att uppgradera mina järnklubbor. Vad rekommenderar ni? Budget är ca 5000-8000 kr.",
                    FkUserId = bjorn.Id,
                    PostCreateDate = DateTime.UtcNow.AddDays(-7),
                    Comments = new List<Comment>
                    {
                        new Comment { CommentContent = "Callaway Rogue-serien är fantastisk för mellannivå, väl värd pengarna!", FkUserId = anna.Id,  CommentDateTime = DateTime.UtcNow.AddDays(-6) },
                        new Comment { CommentContent = "Titleist T300 är också ett bra alternativ, prova dem i butik först.", FkUserId = david.Id, CommentDateTime = DateTime.UtcNow.AddDays(-5) },
                    }
                },
                new Post
                {
                    PostTitle = "Vad tycker ni om Golfklubb Centar? ⛳",
                    PostContent = "Har precis blivit medlem och är jättenöjd! Banan är välskött och personalen är trevlig. Nån annan här som är ny?",
                    FkUserId = cecilia.Id,
                    PostCreateDate = DateTime.UtcNow.AddDays(-3),
                    Comments = new List<Comment>
                    {
                        new Comment { CommentContent = "Välkommen! Ja, jag gick med förra månaden. Banan på hål 7 är min favorit 🌿", FkUserId = anna.Id,  CommentDateTime = DateTime.UtcNow.AddDays(-2) },
                        new Comment { CommentContent = "Håller med! Driving rangen är också toppen.", FkUserId = bjorn.Id, CommentDateTime = DateTime.UtcNow.AddDays(-1) },
                    }
                },
                new Post
                {
                    PostTitle = "Golfresa till Spanien — någon intresserad?",
                    PostContent = "Planerar en golfresa till Costa del Sol i september. Finns det intresse för att åka ihop? Vi är 4 personer hittills. 🌞⛳",
                    FkUserId = david.Id,
                    PostCreateDate = DateTime.UtcNow.AddDays(-1),
                }
            };

            context.Posts.AddRange(posts);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar recensioner på produkter.
        /// </summary>
        public static async Task SeedReviews(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var anna = await userManager.FindByEmailAsync("anna@test.se");
            var bjorn = await userManager.FindByEmailAsync("bjorn@test.se");
            var cecilia = await userManager.FindByEmailAsync("cecilia@test.se");
            var david = await userManager.FindByEmailAsync("david@test.se");

            if (anna == null || bjorn == null) return;

            var products = context.Products.ToList();
            if (!products.Any()) return;

            var reviews = new List<ProductReview>
            {
                new ProductReview
                {
                    FkProductId = products[0].ProductId,
                    FkUserId = anna.Id,
                    ProductReviewContent = "Fantastisk driver! Märker tydlig skillnad i distansen. Rekommenderas varmt! 🏌️‍♀️",
                    Rating = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
                },
                new ProductReview
                {
                    FkProductId = products[0].ProductId,
                    FkUserId = bjorn.Id,
                    ProductReviewContent = "Bra driver men lite dyr. Kvaliteten är dock outstanding.",
                    Rating = 4,
                    CreatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new ProductReview
                {
                    FkProductId = products[1].ProductId,
                    FkUserId = cecilia.Id,
                    ProductReviewContent = "Callaway Rogue är precis vad jag behövde. Järnklubbornas balans är perfekt.",
                    Rating = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new ProductReview
                {
                    FkProductId = products[7].ProductId,
                    FkUserId = david.Id,
                    ProductReviewContent = "Supersoft-bollarna är verkligen mjuka och ger bra distans. Köper igen!",
                    Rating = 4,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new ProductReview
                {
                    FkProductId = products[9].ProductId,
                    FkUserId = anna.Id,
                    ProductReviewContent = "FootJoy Pro SL är de bekvämbaste golfskor jag haft. Vattentätheten håller!",
                    Rating = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
            };

            context.ProductReviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }
    }
}