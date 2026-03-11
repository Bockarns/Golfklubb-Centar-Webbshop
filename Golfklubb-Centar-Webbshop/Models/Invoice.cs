using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Invoices", Schema = "CentarOrderMngt")]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    public string FkUserId { get; set; } = null!;

    [Column("FK_OrderId")]
    public int FkOrderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDateTime { get; set; }

    [Column("FK_PaymentId")]
    public int? FkPaymentId { get; set; }

    [Column("FK_TaxId")]
    public int FkTaxId { get; set; }

    [ForeignKey("FkOrderId")]
    [InverseProperty("Invoices")]
    public virtual Order FkOrder { get; set; } = null!;

    [ForeignKey("FkPaymentId")]
    [InverseProperty("Invoices")]
    public virtual Payment? FkPayment { get; set; }

    [ForeignKey("FkTaxId")]
    [InverseProperty("Invoices")]
    public virtual Taxis FkTax { get; set; } = null!;

    [ForeignKey("FkUserId")]
    [InverseProperty("Invoices")]
    public ApplicationUser FkUser { get; set; } = null!;

    [InverseProperty("FkInvoice")]
    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    [InverseProperty("FkInvoice")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
