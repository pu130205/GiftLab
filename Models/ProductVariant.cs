namespace GiftLab.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; } 
        public string Name { get; set; } = ""; 

        public string ImagePath { get; set; } = ""; 
        public decimal Price { get; set; } 
        public decimal? OriginalPrice { get; set; }

        public string StockStatus { get; set; } = "";
        public int StockQuantity { get; set; }

        public void SyncStockStatus()
        {
            StockStatus = StockQuantity > 0 ? "Còn hàng" : "Hết hàng";
        }

        public bool IsAvailable => StockQuantity > 0;
    }
}