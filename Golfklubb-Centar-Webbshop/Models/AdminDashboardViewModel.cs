namespace Golfklubb_Centar_Webbshop.Models
{
    public class AdminDashboardViewModel
    {
        // Orderstatus-counters
        public int OrdersPending { get; set; }
        public int OrdersProcessing { get; set; }
        public int OrdersShipped { get; set; }
        public int OrdersDelivered { get; set; }
        public int OrdersCancelled { get; set; }

        // Forum-counters
        public int NewPosts { get; set; }
        public int NewComments { get; set; }

        // Review-counter
        public int NewReviews { get; set; }

        // Senaste ordrar
        public List<Order> LatestOrders { get; set; } = new();

        // Senaste trådar
        public List<Post> LatestPosts { get; set; } = new();
    }
}