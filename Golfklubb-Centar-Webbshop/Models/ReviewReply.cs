using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models
{
    [Table("ReviewReplies", Schema = "CentarProductMngt")]
    public class ReviewReply
    {
        [Key]
        public int ReviewReplyId { get; set; }

        [StringLength(500)]
        [Unicode(false)]
        public string ReplyContent { get; set; } = null!;

        [Column("FK_ProductReviewId")]
        public int FkProductReviewId { get; set; }

        [Column("FK_UserId")]
        [StringLength(450)]
        public string FkUserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        [ForeignKey("FkProductReviewId")]
        [InverseProperty("Replies")]
        public virtual ProductReview FkProductReview { get; set; } = null!;

        [ForeignKey("FkUserId")]
        public ApplicationUser FkUser { get; set; } = null!;
    }
}
