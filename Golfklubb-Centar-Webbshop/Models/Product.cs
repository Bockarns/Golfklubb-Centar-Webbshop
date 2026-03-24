using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Products", Schema = "CentarProductMngt")]
public partial class Product
{
    [Key]
    public int ProductId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "decimal(7, 2)")]
    public decimal ProductPrice { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ProductDescribtion { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ProductImgPath { get; set; }

    [Column("FK_CategoryId")]
    [Required(ErrorMessage = "Välj kategori")]
    public int FkCategoryId { get; set; }

    [Column("FK_DiscountId")]
    [Required(ErrorMessage = "Välj rabatt")]
    public int FkDiscountId { get; set; }

    [InverseProperty("FkProduct")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("FkCategoryId")]
    [InverseProperty("Products")]
    public virtual Category FkCategory { get; set; } = null!;

    [ForeignKey("FkDiscountId")]
    [InverseProperty("Products")]
    public virtual Discount FkDiscount { get; set; } = null!;

    [InverseProperty("FkProduct")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("FkProduct")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    [InverseProperty("FkProduct")]
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
