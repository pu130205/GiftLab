namespace GiftLab.Models
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }

        public string Name { get; set; } = "";
        public string ImagePath { get; set; } = "";

        public int UnitPrice { get; set; }
        public int Quantity { get; set; }

        public int LineTotal => UnitPrice * Quantity;
    }
}
