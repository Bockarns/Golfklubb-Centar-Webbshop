namespace Golfklubb_Centar_Webbshop.Models
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; } = new Product();

        public List<Category> Categories { get; set; } = new();
        public List<Discount> Discounts { get; set; } = new();
    }
}
