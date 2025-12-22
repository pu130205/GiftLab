namespace GiftLab.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Category { get; set; } = "";

        public string ImagePath { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public string Description { get; set; } = "";
        public int Price { get; set; }
        public int? OriginalPrice { get; set; }

        public double Rating { get; set; }
        public int SoldCount { get; set; }      // dùng để tính sp bán chạy

        // 🚨 THÊM THUỘC TÍNH NÀY: Danh sách các biến thể/phân loại
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        public bool IsOnSale =>
            OriginalPrice.HasValue && OriginalPrice > Price;
    }
}