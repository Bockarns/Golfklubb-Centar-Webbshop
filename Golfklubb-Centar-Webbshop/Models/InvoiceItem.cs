using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("InvoiceItems", Schema = "CentarOrderMngt")]
public partial class InvoiceItem
{
    [Key]
    public int InvoiceItemId { get; set; }

    [Column("FK_InvoiceId")]
    public int FkInvoiceId { get; set; }

    [Column("FK_ProductId")]
    public int FkProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal UnitPrice { get; set; }

    [Column("FK_DiscountId")]
    public int? FkDiscountId { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal SubTotal { get; set; }

    [ForeignKey("FkInvoiceId")]
    [InverseProperty("InvoiceItems")]
    public virtual Invoice FkInvoice { get; set; } = null!;

    [ForeignKey("FkProductId")]
    [InverseProperty("InvoiceItems")]
    public virtual Product FkProduct { get; set; } = null!;
}
