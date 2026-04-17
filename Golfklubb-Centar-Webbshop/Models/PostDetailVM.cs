namespace Golfklubb_Centar_Webbshop.Models
{
    public class PostDetailVM
    {
        public Post Post { get; set; } = new();
        public CommentPagination CommentPagination { get; set; } = new();
    }
}
