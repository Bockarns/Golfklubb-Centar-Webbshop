using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
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
    [Unicode(false)]
    public string PostTitle { get; set; } = null!;

    [StringLength(1000)]
    [Unicode(false)]
    public string PostContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime PostCreateDate { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    public string FkUserId { get; set; } = null!;

    [InverseProperty("FkPost")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [ForeignKey("FkUserId")]
    [InverseProperty("Posts")]
    public ApplicationUser FkUser { get; set; } = null!;
}
