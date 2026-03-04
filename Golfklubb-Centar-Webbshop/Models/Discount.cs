using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Discounts", Schema = "CentarProductMngt")]
public partial class Discount
{
    [Key]
    public int DiscountId { get; set; }

    [Column("Discount")]
    public int Discount1 { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string DiscountDescribtion { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string DiscountType { get; set; } = null!;

    [InverseProperty("FkDiscount")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
