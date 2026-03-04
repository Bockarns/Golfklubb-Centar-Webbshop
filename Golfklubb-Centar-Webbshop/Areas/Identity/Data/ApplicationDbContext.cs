using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Golfklubb_Centar_Webbshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Taxis> Taxes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        RenameIdentityTables(builder);

        builder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK_CartId");
        });

        builder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK_CartItemId");

            entity.HasOne(d => d.FkCart).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_CartId");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItems_ProductId");
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_CategoryId");

            entity.HasOne(d => d.FkParentCategory).WithMany(p => p.InverseFkParentCategory).HasConstraintName("FK_Categories_ParentCategoryId");
        });

        builder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK_CommentId");

            entity.HasOne(d => d.FkPost).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_PostId");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_UserId");
        });

        builder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK_DiscountId");
        });

        builder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.OrderHistoryId).HasName("PK_OrderHistoryId");

            entity.HasOne(d => d.FkInvoice).WithMany(p => p.Histories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Histories_InvoiceId");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Histories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Histories_UserId");
        });

        builder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK_InvoiceId");

            entity.HasOne(d => d.FkOrder).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_OrderId");

            entity.HasOne(d => d.FkPayment).WithMany(p => p.Invoices).HasConstraintName("FK_Invoices_PaymentId");

            entity.HasOne(d => d.FkTax).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_TaxId");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_UserId");
        });

        builder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.InvoiceItemId).HasName("PK_InvoiceItemId");

            entity.HasOne(d => d.FkInvoice).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceItems_InvoiceId");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.InvoiceItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoiceItems_ProductId");
        });

        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_OrderId");

            entity.HasOne(d => d.FkCart).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_CartId");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_UserId");
        });

        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK_PaymentId");

            entity.HasOne(d => d.FkOrder).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_OrderId");
        });

        builder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK_PostId");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_UserId");
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_ProductID");

            entity.HasOne(d => d.FkCategory).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_CategoryId");

            entity.HasOne(d => d.FkDiscount).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_DiscountId");
        });

        builder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ProductReviewId).HasName("PK_ProductReviewId");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.ProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_ProductId");

            entity.HasOne(d => d.FkUser).WithMany(p => p.ProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductReviews_UserId");
        });

        

        builder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("PK_StockId");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.Stocks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stocks_ProductId");
        });

        builder.Entity<Taxis>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("PK_TaxId");
        });

        

        OnModelCreatingPartial(builder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    protected void RenameIdentityTables(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("CentarUserMngt");
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(name: "Users");
        });
        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Roles");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable(name: "UserRoles");
        });
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable(name: "UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable(name: "UserLogin");
        });
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable(name: "RoleClaims");
        });
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable(name: "UserTokens");
        });

    }
}
