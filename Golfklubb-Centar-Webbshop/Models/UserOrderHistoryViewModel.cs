namespace Golfklubb_Centar_Webbshop.Models
{
    public class UserOrderHistoryViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}
