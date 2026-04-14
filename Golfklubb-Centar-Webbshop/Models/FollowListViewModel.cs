namespace Golfklubb_Centar_Webbshop.Models
{
    public class FollowListViewModel
    {
        public string TargetUserId { get; set; }
        public string TargetUserName { get; set; }
        public bool IsOwnProfile { get; set; }
        public List<Follow> Follows { get; set; } = new();
    }
}