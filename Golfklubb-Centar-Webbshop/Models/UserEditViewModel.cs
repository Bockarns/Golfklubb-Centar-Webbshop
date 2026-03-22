namespace Golfklubb_Centar_Webbshop.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
