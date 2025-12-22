using System.ComponentModel.DataAnnotations;

namespace GiftLab.Models
{
    public class CheckoutViewModel
    {
        // Khớp Customer/Order
        [Required] public string FullName { get; set; } = "";
        [Required] public string Phone { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string City { get; set; } = "";
        [Required] public string Ward { get; set; } = "";
        [Required] public string Address { get; set; } = "";
        public string? Note { get; set; } // map Order.Note

        // Payment (map Order.PaymentID sau)
        public string PaymentMethod { get; set; } = "COD"; // COD/BANK

        // Items
        public List<CheckoutItemVM> Items { get; set; } = new();

        // Tổng tiền
        public decimal ShippingFee { get; set; } = 30000; // ✅ decimal
        public decimal Subtotal => Items.Sum(x => x.LineTotal);
        public decimal GrandTotal => Subtotal + ShippingFee;

        public string Mode { get; set; } = "CART";
        
    }
}
