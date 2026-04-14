using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Posts", Schema = "CentarForumMngt")]
public partial class Post
{
    [Key]
    public int PostId { get; set; }

    [StringLength(50)]
    public string PostTitle { get; set; } = null!;

    [StringLength(1000)]
    public string PostContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime PostCreateDate { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    [ValidateNever]
    public string FkUserId { get; set; } = null!;

    [InverseProperty("FkPost")]
    [ValidateNever]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [ForeignKey("FkUserId")]
    [InverseProperty("Posts")]
    [ValidateNever]
    public ApplicationUser FkUser { get; set; } = null!;
}
