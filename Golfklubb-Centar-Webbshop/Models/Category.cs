using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    public int? FkParentCategoryId { get; set; }

    [ForeignKey("FkParentCategoryId")]
    [InverseProperty("InverseFkParentCategory")]
    public virtual Category? FkParentCategory { get; set; }

    [InverseProperty("FkParentCategory")]
    public virtual ICollection<Category> InverseFkParentCategory { get; set; } = new List<Category>();

    [InverseProperty("FkCategory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
