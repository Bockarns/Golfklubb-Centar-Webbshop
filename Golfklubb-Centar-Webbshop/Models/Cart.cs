using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Carts", Schema = "CentarOrderMngt")]
public partial class Cart
{
    [Key]
    public int CartId { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal TotalPrice { get; set; }

    [InverseProperty("FkCart")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("FkCart")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
