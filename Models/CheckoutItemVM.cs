namespace GiftLab.Models
{
    public class CheckoutItemVM
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string Name { get; set; } = "";
        public string ImagePath { get; set; } = "";

        public decimal UnitPrice { get; set; }    
        public int Quantity { get; set; }

        public decimal LineTotal => UnitPrice * Quantity; 
    }
}
