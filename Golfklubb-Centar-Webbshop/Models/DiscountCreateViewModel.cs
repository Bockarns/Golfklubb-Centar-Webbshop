using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Golfklubb_Centar_Webbshop.Models
{
    public class DiscountCreateViewModel
    {
        public Discount Discount { get; set; } = new Discount();
    }
}