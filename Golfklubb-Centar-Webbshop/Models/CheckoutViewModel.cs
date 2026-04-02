using System.ComponentModel.DataAnnotations;

namespace Golfklubb_Centar_Webbshop.Models
{
    public class CheckoutViewModel
    {
        [Required]
        [StringLength(50)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = null!;
    }
}
