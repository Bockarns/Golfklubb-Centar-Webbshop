using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Taxes", Schema = "CentarOrderMngt")]
public partial class Taxis
{
    [Key]
    public int TaxId { get; set; }

    public int TaxIndex { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TaxDescribtion { get; set; } = null!;

    [InverseProperty("FkTax")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
