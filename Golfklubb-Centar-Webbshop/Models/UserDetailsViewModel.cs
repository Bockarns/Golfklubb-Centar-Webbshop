namespace Golfklubb_Centar_Webbshop.Models
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsForumBanned { get; set; }
        public IList<string> Roles { get; set; }
        public string ProfileImageUrl { get; set; }
        public string FullName { get; set; }
    }
}