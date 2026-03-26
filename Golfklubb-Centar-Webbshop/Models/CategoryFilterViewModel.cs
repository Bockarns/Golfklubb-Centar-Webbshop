namespace Golfklubb_Centar_Webbshop.Models
{
    public class CategoryFilterViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> ParentCategories { get; set; } = new();
        public int? SelectedCategory { get; set; }
    }
}
