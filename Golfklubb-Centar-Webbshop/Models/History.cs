using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Histories", Schema = "CentarOrderMngt")]
public partial class History
{
    [Key]
    public int OrderHistoryId { get; set; }

    [Column("FK_InvoiceId")]
    public int FkInvoiceId { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    public string FkUserId { get; set; } = null!;

    [ForeignKey("FkInvoiceId")]
    [InverseProperty("Histories")]
    public virtual Invoice FkInvoice { get; set; } = null!;

    [ForeignKey("FkUserId")]
    [InverseProperty("Histories")]
    public ApplicationUser FkUser { get; set; } = null!;
}
