using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Identity;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Data
{
    public class SeedData
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@test.se";
            var adminPassword = "Test123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if(adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newAdmin, adminPassword);
                await userManager.AddToRoleAsync(newAdmin, "Admin");
               
            }
        }

        public static async Task SeedDiscount(ApplicationDbContext context)
        {
            var discounts = new List<Discount>
            {
                new Discount
                {
                    Discount1 = 0,
                    DiscountDescribtion ="Ordinarie Pris",
                    DiscountType = "%"
                },
                new Discount
                {
                    Discount1 = 10,
                    DiscountDescribtion = "StartRea",
                    DiscountType = "%"
                }
            };
            context.Discounts.AddRange(discounts);
            await context.SaveChangesAsync();
        }
        
        public static async Task SeedCategory(ApplicationDbContext context)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "TestCatagory1"

                },
                new Category
                {
                    CategoryName = "TestCategory2"
                }

            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        public static async Task SeedProduct(ApplicationDbContext context)
        {
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = "Keps Grön",
                    ProductPrice = 249.99M,
                    ProductDescribtion = "En grön keps",
                    FkCategoryId = 1,
                    FkDiscountId = 1,
                },
                new Product
                {
                    ProductName = "Handske Blå",
                    ProductPrice = 199.49M,
                    ProductDescribtion = "En blå golfhandske",
                    FkCategoryId = 2,
                    FkDiscountId = 1,
                },
                new Product
                {
                    ProductName = "Golfbag vit",
                    ProductPrice = 1999.99M,
                    ProductDescribtion = "En vit golfbag för 15 klubbor",
                    FkCategoryId = 1,
                    FkDiscountId = 2,
                }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}
