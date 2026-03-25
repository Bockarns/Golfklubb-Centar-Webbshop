namespace Golfklubb_Centar_Webbshop.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Stocks { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductDescribtion { get; set; }
        public string? ProductImgPath { get; set; }
        public int FkCategoryId { get; set; }
        public int FkDiscountId { get; set; }
        public string? CategoryName { get; set; }
        public string? DiscountDescription { get; set; }
    }
}
