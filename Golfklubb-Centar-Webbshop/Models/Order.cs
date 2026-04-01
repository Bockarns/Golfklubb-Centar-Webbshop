using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Orders", Schema = "CentarOrderMngt")]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    public string FkUserId { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? OrderStatus { get; set; }

    [StringLength(50)]
    public string Address { get; set; } = null!;

    [StringLength(10)]
    public string PostalCode { get; set; } = null!;

    [StringLength(20)]
    public string City { get; set; } = null!;

    [StringLength(50)]
    public string Country { get; set; } = null!;

    [StringLength(20)]
    public string Phone { get; set; } = null!;

    [ForeignKey("FkUserId")]
    [InverseProperty("Orders")]
    public ApplicationUser FkUser { get; set; } = null!;

    [InverseProperty("FkOrder")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("FkOrder")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
