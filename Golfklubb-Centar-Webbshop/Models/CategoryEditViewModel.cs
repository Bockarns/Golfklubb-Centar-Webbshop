using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Golfklubb_Centar_Webbshop.Models
{
    public class CategoryEditViewModel
    {
        public Category Category { get; set; } = new Category();

        [ValidateNever]
        public List<Category> ParentCategories { get; set; } = new();
    }
}
