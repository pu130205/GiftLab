namespace GiftLab.Models
{
        public class ProductCardViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string Category { get; set; } = "";
            public string ImagePath { get; set; } = "";
            public int? Price { get; set; }
            public int? OriginalPrice { get; set; }
            public double Rating { get; set; }
            public int SoldCount { get; set; }
            public bool IsOnSale => OriginalPrice.HasValue && OriginalPrice > Price;
    }

}
