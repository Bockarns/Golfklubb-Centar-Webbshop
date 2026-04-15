using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Golfklubb_Centar_Webbshop.Models
{
    [Table("Follows", Schema = "CentarUserMngt")]
    public class Follow
    {
        [Key]
        public int FollowId { get; set; }
        [Column("FK_UserId")]
        [StringLength(450)]
        public string FkUserId { get; set; } = null!;
        [Column("FK_FollowedUserId")]
        [StringLength(450)]
        public string FkFollowedUserId { get; set; } = null!;
        public DateTime FollowDate { get; set; } = DateTime.UtcNow;
        [ForeignKey("FkUserId")]
        public virtual ApplicationUser FkUser { get; set; } = null!;
        [ForeignKey("FkFollowedUserId")]
        public virtual ApplicationUser FkFollowedUser { get; set; } = null!;
    }
}
