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
            var klader = new Category { CategoryName = "Kläder" };
            var tillbehor = new Category { CategoryName = "Tillbehör" };
            var accessoar = new Category { CategoryName = "Accessoarer" };

            context.Categories.AddRange(klader, tillbehor, accessoar);
            await context.SaveChangesAsync();

            // Underkategorier
            var underkategorier = new List<Category>
    {
        new Category { CategoryName = "Pikétröjor",  FkParentCategoryId = klader.CategoryId },
        new Category { CategoryName = "Kepsar",      FkParentCategoryId = klader.CategoryId },
        new Category { CategoryName = "Handskar",    FkParentCategoryId = klader.CategoryId },
        new Category { CategoryName = "Golfbagar",   FkParentCategoryId = tillbehor.CategoryId },
        new Category { CategoryName = "Golfbollar",  FkParentCategoryId = tillbehor.CategoryId },
        new Category { CategoryName = "Flaskor",     FkParentCategoryId = tillbehor.CategoryId },
        new Category { CategoryName = "Muggar",      FkParentCategoryId = accessoar.CategoryId },
        new Category { CategoryName = "Termosar",    FkParentCategoryId = accessoar.CategoryId },
        new Category { CategoryName = "Tote Bags",   FkParentCategoryId = accessoar.CategoryId },
        new Category { CategoryName = "Fodral",      FkParentCategoryId = accessoar.CategoryId },
    };

            context.Categories.AddRange(underkategorier);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Skapar exempelprodukter kopplade till kategorier och rabatter.
        /// </summary>
        public static async Task SeedProduct(ApplicationDbContext context)
        {
            var piketrojor = context.Categories.First(c => c.CategoryName == "Pikétröjor").CategoryId;
            var kepsar = context.Categories.First(c => c.CategoryName == "Kepsar").CategoryId;
            var handskar = context.Categories.First(c => c.CategoryName == "Handskar").CategoryId;
            var golfbagar = context.Categories.First(c => c.CategoryName == "Golfbagar").CategoryId;
            var golfbollar = context.Categories.First(c => c.CategoryName == "Golfbollar").CategoryId;
            var flaskor = context.Categories.First(c => c.CategoryName == "Flaskor").CategoryId;
            var muggar = context.Categories.First(c => c.CategoryName == "Muggar").CategoryId;
            var termosar = context.Categories.First(c => c.CategoryName == "Termosar").CategoryId;
            var totebags = context.Categories.First(c => c.CategoryName == "Tote Bags").CategoryId;
            var fodral = context.Categories.First(c => c.CategoryName == "Fodral").CategoryId;

            var ordinarie = context.Discounts.First(d => d.Discount1 == 0).DiscountId;
            var rea10 = context.Discounts.First(d => d.Discount1 == 10).DiscountId;
            var rea20 = context.Discounts.First(d => d.Discount1 == 20).DiscountId;

            var products = new List<Product>
    {
        // Pikétröjor
        new Product { ProductName = "Pikétröja Svart",  ProductPrice = 499.00M, ProductDescribtion = "Klassisk pikétröja med klubblogga, perfekt för en dag på banan.",          FkCategoryId = piketrojor, FkDiscountId = ordinarie, ProductImgPath = "/images/products/Tshirt1.png" },
        new Product { ProductName = "Pikétröja Vit",    ProductPrice = 499.00M, ProductDescribtion = "Fräsch vit pikétröja med broderad klubblogga.",                             FkCategoryId = piketrojor, FkDiscountId = rea10,     ProductImgPath = "/images/products/Tshirt2.png" },
        new Product { ProductName = "Pikétröja Blå",    ProductPrice = 499.00M, ProductDescribtion = "Ljusblå pikétröja i andningsbart material med klubblogga.",                 FkCategoryId = piketrojor, FkDiscountId = ordinarie, ProductImgPath = "/images/products/Tshirt3.png" },

        // Kepsar
        new Product { ProductName = "Keps Vit",         ProductPrice = 299.00M, ProductDescribtion = "Vit keps med justerbart band och broderad klubblogga.",                     FkCategoryId = kepsar,     FkDiscountId = ordinarie, ProductImgPath = "/images/products/Keps1.png" },
        new Product { ProductName = "Keps Blå",         ProductPrice = 299.00M, ProductDescribtion = "Ljusblå keps med UV-skydd och broderad klubblogga.",                        FkCategoryId = kepsar,     FkDiscountId = rea10,     ProductImgPath = "/images/products/Keps2.png" },
        new Product { ProductName = "Keps Svart",       ProductPrice = 299.00M, ProductDescribtion = "Svart keps med justerbart band och broderad klubblogga.",                   FkCategoryId = kepsar,     FkDiscountId = ordinarie, ProductImgPath = "/images/products/Keps3.png" },

        // Handskar
        new Product { ProductName = "Handske Vit",      ProductPrice = 199.00M, ProductDescribtion = "Vit golfhandske med klubblogga för bättre grepp och komfort.",              FkCategoryId = handskar,   FkDiscountId = ordinarie, ProductImgPath = "/images/products/Handskar1.png" },
        new Product { ProductName = "Handske Svart",    ProductPrice = 199.00M, ProductDescribtion = "Svart golfhandske med klubblogga, slitstark och bekväm.",                   FkCategoryId = handskar,   FkDiscountId = rea10,     ProductImgPath = "/images/products/Handskar2.png" },
        new Product { ProductName = "Handske Grön",     ProductPrice = 199.00M, ProductDescribtion = "Grön golfhandske med klubblogga, slitstark och bekväm.",                    FkCategoryId = handskar,   FkDiscountId = ordinarie, ProductImgPath = "/images/products/Handskar3.png" },

        // Golfbagar
        new Product { ProductName = "Golfbag Beige",    ProductPrice = 2499.00M, ProductDescribtion = "Rymlig golfbag i beige med flera fack och broderad klubblogga.",           FkCategoryId = golfbagar,  FkDiscountId = ordinarie, ProductImgPath = "/images/products/7.png" },
        new Product { ProductName = "Golfbag Svart",    ProductPrice = 2499.00M, ProductDescribtion = "Stilren svart golfbag med flera fack och broderad klubblogga.",             FkCategoryId = golfbagar,  FkDiscountId = rea10,     ProductImgPath = "/images/products/8.png" },
        new Product { ProductName = "Golfbag Grön",     ProductPrice = 2499.00M, ProductDescribtion = "Grön golfbag med flera fack och broderad klubblogga.",                     FkCategoryId = golfbagar,  FkDiscountId = rea20,     ProductImgPath = "/images/products/9.png" },

        // Golfbollar
        new Product { ProductName = "Golfboll",         ProductPrice = 149.00M, ProductDescribtion = "Golfboll med klubblogga, säljs styckvis.",                                  FkCategoryId = golfbollar, FkDiscountId = ordinarie, ProductImgPath = "/images/products/Golfball.png" },

        // Flaskor
        new Product { ProductName = "Flaska Silver",    ProductPrice = 349.00M, ProductDescribtion = "Återanvändbar aluminiumflaska med klubblogga, håller drycken kall.",        FkCategoryId = flaskor,    FkDiscountId = ordinarie, ProductImgPath = "/images/products/Flaska1.png" },
        new Product { ProductName = "Flaska Svart",     ProductPrice = 349.00M, ProductDescribtion = "Svart återanvändbar flaska med klubblogga, håller drycken kall.",           FkCategoryId = flaskor,    FkDiscountId = rea10,     ProductImgPath = "/images/products/Flaska2.png" },
        new Product { ProductName = "Flaska Vit",       ProductPrice = 349.00M, ProductDescribtion = "Vit återanvändbar flaska med klubblogga, håller drycken kall.",             FkCategoryId = flaskor,    FkDiscountId = ordinarie, ProductImgPath = "/images/products/Flaska3.png" },

        // Muggar
        new Product { ProductName = "Mugg Vit",         ProductPrice = 199.00M, ProductDescribtion = "Keramisk mugg med klubblogga, perfekt för kaffet efter rundan.",            FkCategoryId = muggar,     FkDiscountId = ordinarie, ProductImgPath = "/images/products/10.png" },

        // Termosar
        new Product { ProductName = "Termos Silver",    ProductPrice = 449.00M, ProductDescribtion = "Rostfri termos med klubblogga, håller drycken varm i timmar.",              FkCategoryId = termosar,   FkDiscountId = ordinarie, ProductImgPath = "/images/products/13.png" },
        new Product { ProductName = "Termos Svart",     ProductPrice = 449.00M, ProductDescribtion = "Svart rostfri termos med klubblogga, håller drycken varm i timmar.",        FkCategoryId = termosar,   FkDiscountId = rea10,     ProductImgPath = "/images/products/14.png" },
        new Product { ProductName = "Termos Beige",     ProductPrice = 449.00M, ProductDescribtion = "Beige rostfri termos med klubblogga, håller drycken varm i timmar.",        FkCategoryId = termosar,   FkDiscountId = ordinarie, ProductImgPath = "/images/products/15.png" },

        // Tote Bags
        new Product { ProductName = "Tote Bag Vit",     ProductPrice = 249.00M, ProductDescribtion = "Rymlig tote bag i vitt med broderad klubblogga.",                          FkCategoryId = totebags,   FkDiscountId = ordinarie, ProductImgPath = "/images/products/16.png" },
        new Product { ProductName = "Tote Bag Svart",   ProductPrice = 249.00M, ProductDescribtion = "Rymlig tote bag i svart med broderad klubblogga.",                         FkCategoryId = totebags,   FkDiscountId = rea10,     ProductImgPath = "/images/products/17.png" },
        new Product { ProductName = "Tote Bag Grön",    ProductPrice = 249.00M, ProductDescribtion = "Rymlig tote bag i grönt med broderad klubblogga.",                         FkCategoryId = totebags,   FkDiscountId = ordinarie, ProductImgPath = "/images/products/18.png" },

        // Fodral
        new Product { ProductName = "Fodral Vit",       ProductPrice = 179.00M, ProductDescribtion = "Smidigt fodral med klubblogga, passar telefon eller tillbehör.",            FkCategoryId = fodral,     FkDiscountId = ordinarie, ProductImgPath = "/images/products/4.png" },
        new Product { ProductName = "Fodral Svart",     ProductPrice = 179.00M, ProductDescribtion = "Smidigt svart fodral med klubblogga, passar telefon eller tillbehör.",      FkCategoryId = fodral,     FkDiscountId = rea10,     ProductImgPath = "/images/products/5.png" },
        new Product { ProductName = "Fodral Grön",      ProductPrice = 179.00M, ProductDescribtion = "Smidigt grönt fodral med klubblogga, passar telefon eller tillbehör.",      FkCategoryId = fodral,     FkDiscountId = ordinarie, ProductImgPath = "/images/products/6.png" },
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

            var piketrojaSvart = context.Products.First(p => p.ProductName == "Pikétröja Svart").ProductId;
            var piketrojaVit = context.Products.First(p => p.ProductName == "Pikétröja Vit").ProductId;
            var golfbagSvart = context.Products.First(p => p.ProductName == "Golfbag Svart").ProductId;
            var golfboll = context.Products.First(p => p.ProductName == "Golfboll").ProductId;
            var flaskaSilver = context.Products.First(p => p.ProductName == "Flaska Silver").ProductId;
            var kepsSvart = context.Products.First(p => p.ProductName == "Keps Svart").ProductId;
            var handskVit = context.Products.First(p => p.ProductName == "Handske Vit").ProductId;
            var termosSvart = context.Products.First(p => p.ProductName == "Termos Svart").ProductId;

            var reviews = new List<ProductReview>
    {
        new ProductReview
        {
            FkProductId = piketrojaSvart,
            FkUserId = anna.Id,
            ProductReviewContent = "Sitter perfekt och materialet är riktigt skönt. Loggan ser proffsig ut! 👕",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        },
        new ProductReview
        {
            FkProductId = piketrojaSvart,
            FkUserId = bjorn.Id,
            ProductReviewContent = "Bra kvalitet men storleken sitter lite stort, ta en storlek mindre.",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        },
        new ProductReview
        {
            FkProductId = piketrojaVit,
            FkUserId = cecilia.Id,
            ProductReviewContent = "Fin tröja! Håller färgen bra efter tvätt. Rekommenderas.",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-7)
        },
        new ProductReview
        {
            FkProductId = golfbagSvart,
            FkUserId = david.Id,
            ProductReviewContent = "Rymlig och snygg bag. Facken är välplacerade och den är lätt att bära.",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-6)
        },
        new ProductReview
        {
            FkProductId = golfboll,
            FkUserId = anna.Id,
            ProductReviewContent = "Snyggt med klubbloggan på bollen! Flyger bra och känns solid.",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        },
        new ProductReview
        {
            FkProductId = flaskaSilver,
            FkUserId = bjorn.Id,
            ProductReviewContent = "Håller drycken kall i flera timmar på banan. Mycket nöjd! 💧",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-4)
        },
        new ProductReview
        {
            FkProductId = kepsSvart,
            FkUserId = cecilia.Id,
            ProductReviewContent = "Sitter bra och skuggar perfekt. Loggan ser snygg ut mot det svarta.",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        },
        new ProductReview
        {
            FkProductId = handskVit,
            FkUserId = david.Id,
            ProductReviewContent = "Bra passform och grepp. Håller bra kvalitet jämfört med priset.",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new ProductReview
        {
            FkProductId = termosSvart,
            FkUserId = anna.Id,
            ProductReviewContent = "Håller kaffet varmt hela rundan! Robust och snygg design. ☕",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        },
    };

            context.ProductReviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }
    }
}