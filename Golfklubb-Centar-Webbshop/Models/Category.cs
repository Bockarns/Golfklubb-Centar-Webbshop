using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Categories", Schema = "CentarProductMngt")]
public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CategoryName { get; set; } = null!;

    [Column("FK_ParentCategoryId")]
    [ValidateNever]
    public int? FkParentCategoryId { get; set; }

    [ForeignKey("FkParentCategoryId")]
    [InverseProperty("InverseFkParentCategory")]
    [ValidateNever]
    public virtual Category? FkParentCategory { get; set; }

    [InverseProperty("FkParentCategory")]
    [ValidateNever]
    public virtual ICollection<Category> InverseFkParentCategory { get; set; } = new List<Category>();

    [InverseProperty("FkCategory")]
    [ValidateNever]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
