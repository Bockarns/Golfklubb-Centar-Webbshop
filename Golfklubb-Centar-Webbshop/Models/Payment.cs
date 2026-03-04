using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Payments", Schema = "CentarOrderMngt")]
public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Column("FK_OrderId")]
    public int FkOrderId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PaymentMethod { get; set; }

    public bool? PaymentStatus { get; set; }

    public int? TransactionId { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal? Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [ForeignKey("FkOrderId")]
    [InverseProperty("Payments")]
    public virtual Order FkOrder { get; set; } = null!;

    [InverseProperty("FkPayment")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
