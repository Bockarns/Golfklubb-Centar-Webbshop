using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Golfklubb_Centar_Webbshop.Models
{
    public class ProductEditViewModel
    {
        public Product Product { get; set; } = new Product();

        public int StockQuantity { get; set; }

        [ValidateNever]
        public List<Category> Categories { get; set; } = new();

        [ValidateNever]
        public List<Discount> Discounts { get; set; } = new();

        public IFormFile? ProductImgPath { get; set; }
        public string? ExistingImgPath { get; set; }
    }
}