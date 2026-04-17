namespace Golfklubb_Centar_Webbshop.Models
{
    public class CommentPagination
    {
        public List<Comment> Comments { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
