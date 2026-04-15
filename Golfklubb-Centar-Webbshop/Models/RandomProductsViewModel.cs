namespace Golfklubb_Centar_Webbshop.Models
{
    public class RandomProductsViewModel
    {
        public Product? Product { get; set; } = null;

        public List<Product>? RandomProducts { get; set; } = new();

        public bool HasUserReviewed { get; set; } = false;

        
    }
}
