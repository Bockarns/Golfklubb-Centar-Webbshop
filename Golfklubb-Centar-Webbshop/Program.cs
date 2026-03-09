using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");;

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                                                    .AddRoles<IdentityRole>()
                                                    .AddEntityFrameworkStores<ApplicationDbContext>()
                                                    .AddDefaultTokenProviders()
                                                    .AddDefaultUI();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await SeedData.SeedRoles(scope.ServiceProvider);
    await SeedData.SeedDiscount(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    await SeedData.SeedCategory(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    await SeedData.SeedProduct(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();


app.Run();
