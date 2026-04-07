using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("OrderItems", Schema = "CentarOrderMngt")]
public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    [Column("FK_OrderId")]
    public int FkOrderId { get; set; }

    [Column("FK_ProductId")]
    public int FkProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal SubTotal { get; set; }

    [ForeignKey("FkOrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order FkOrder { get; set; } = null!;

    [ForeignKey("FkProductId")]
    [InverseProperty("OrderItems")]
    public virtual Product FkProduct { get; set; } = null!;
}