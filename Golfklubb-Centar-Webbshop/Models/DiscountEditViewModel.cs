using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Golfklubb_Centar_Webbshop.Models
{
    public class DiscountEditViewModel
    {
        public Discount Discount { get; set; } = new Discount();

        [ValidateNever]
        public List<Product> Products { get; set; } = new();
    }
}