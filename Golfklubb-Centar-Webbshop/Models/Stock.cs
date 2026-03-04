using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Stocks", Schema = "CentarProductMngt")]
public partial class Stock
{
    [Key]
    public int StockId { get; set; }

    public int Quantity { get; set; }

    [Column("FK_ProductId")]
    public int FkProductId { get; set; }

    [ForeignKey("FkProductId")]
    [InverseProperty("Stocks")]
    public virtual Product FkProduct { get; set; } = null!;
}
