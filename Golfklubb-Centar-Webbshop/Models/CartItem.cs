namespace Golfklubb_Centar_Webbshop.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductImgPath { get; set; } = string.Empty;
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
