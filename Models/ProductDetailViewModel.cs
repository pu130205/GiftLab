using System.Collections.Generic;

namespace GiftLab.Models
{
    public class ProductDetailViewModel
    {
        // Thông tin sản phẩm chính
        public ProductViewModel ProductInfo { get; set; } = new ProductViewModel();

        // Mô tả
        public string ShortDescription { get; set; } = "";
        public string FullDescription { get; set; } = "";

        // Danh sách biến thể
        public List<ProductVariant> Variants { get; set; } = new();

        // Breadcrumb
        public string Breadcrumb { get; set; } = "";

        // Trạng thái
        public bool IsFavorite { get; set; }

        // Biến thể mặc định
        public int DefaultVariantId { get; set; }

        // ❗ SẢN PHẨM LIÊN QUAN → ViewModel, KHÔNG PHẢI ENTITY
        public List<ProductCardViewModel> RelatedProducts { get; set; } = new();
    }
}
