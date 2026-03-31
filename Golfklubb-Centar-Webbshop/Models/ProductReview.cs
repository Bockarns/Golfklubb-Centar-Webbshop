using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("ProductReviews", Schema = "CentarProductMngt")]
public partial class ProductReview
{
    [Key]
    public int ProductReviewId { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ProductReviewContent { get; set; } = null!;

    [Column("FK_ProductId")]
    public int FkProductId { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    public string FkUserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public int Rating { get; set; }

    [ForeignKey("FkProductId")]
    [InverseProperty("ProductReviews")]
    public virtual Product FkProduct { get; set; } = null!;

    [ForeignKey("FkUserId")]
    [InverseProperty("ProductReviews")]
    public ApplicationUser FkUser { get; set; } = null!;
}
