namespace Golfklubb_Centar_Webbshop.Models
{
    public class UserForumHistoryViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}