using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("CartItems", Schema = "CentarOrderMngt")]
public partial class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    public int Quantity { get; set; }

    public DateOnly AddedDate { get; set; }

    [Column("FK_CartId")]
    public int FkCartId { get; set; }

    [Column("FK_ProductId")]
    public int FkProductId { get; set; }

    [ForeignKey("FkCartId")]
    [InverseProperty("CartItems")]
    public virtual Cart FkCart { get; set; } = null!;

    [ForeignKey("FkProductId")]
    [InverseProperty("CartItems")]
    public virtual Product FkProduct { get; set; } = null!;
}
