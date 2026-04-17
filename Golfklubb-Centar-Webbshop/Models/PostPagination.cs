namespace Golfklubb_Centar_Webbshop.Models
{
    public class PostPagination
    {
        public List<Post> Posts { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
