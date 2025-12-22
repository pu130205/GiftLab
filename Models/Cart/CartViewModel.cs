using GiftLab.Models;

namespace GiftLab.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Controller set
        public int ShippingFee { get; set; }

        public int Subtotal => Items?.Sum(x => x.LineTotal) ?? 0;

        public int GrandTotal => Subtotal + ShippingFee;
    }
}
