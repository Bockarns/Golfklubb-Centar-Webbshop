using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models;

[Table("Comments", Schema = "CentarForumMngt")]
public partial class Comment
{
    [Key]
    public int CommentId { get; set; }

    [StringLength(500)]
    public string CommentContent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CommentDateTime { get; set; }

    [Column("FK_UserId")]
    [StringLength(450)]
    public string FkUserId { get; set; } = null!;

    [Column("FK_PostId")]
    public int FkPostId { get; set; }

    [ForeignKey("FkPostId")]
    [InverseProperty("Comments")]
    public virtual Post FkPost { get; set; } = null!;

    [ForeignKey("FkUserId")]
    [InverseProperty("Comments")]
    public ApplicationUser FkUser { get; set; } = null!;
}
