using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golfklubb_Centar_Webbshop.Models
{
    [Table("Notifications", Schema = "CentarUserMngt")]
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        [Column("FK_UserId")]
        [StringLength(450)]
        public string FkUserId { get; set; } = null!;
        [Column("FK_CreatorUserId")]
        [StringLength(450)]
        public string FkCreatorUserId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("FkUserId")]
        public virtual ApplicationUser FkUser { get; set; } = null!;
        [ForeignKey("FkCreatorUserId")]
        public virtual ApplicationUser FkCreatorUser { get; set; } = null!;
    }
}
