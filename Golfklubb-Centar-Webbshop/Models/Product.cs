using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [ForeignKey("FkCategoryId")]
    [InverseProperty("Products")]
    [ValidateNever] //För att kunna spara produkt behövde jag lägga till denna + den nedanför för att ignora objektet och endast Id kan läggas till
    public virtual Category FkCategory { get; set; } = null!;

    [ForeignKey("FkDiscountId")]
    [InverseProperty("Products")]
    [ValidateNever] //För att kunna spara produkt behövde jag lägga till denna för att ignora objektet och endast Id kan läggas till
    public virtual Discount FkDiscount { get; set; } = null!;

    [InverseProperty("FkProduct")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("FkProduct")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    [InverseProperty("FkProduct")]
    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    [InverseProperty("FkProduct")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
