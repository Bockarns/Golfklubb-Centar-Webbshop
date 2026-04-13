namespace Golfklubb_Centar_Webbshop.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public List<Post> Posts { get; set; } = new();
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollower { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
