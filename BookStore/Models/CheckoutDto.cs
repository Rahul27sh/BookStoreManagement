using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "The dilivery Address is required.")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
