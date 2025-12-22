using GiftLab.Models;
using System.Collections.Generic;

namespace GiftLab.ViewModels
{
    public class ShopIndexViewModel
    {
        public List<ProductCardViewModel> Products { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        // 🔹 Danh mục để hiển thị filter
        public List<CategoryViewModel> Categories { get; set; } = new();

        // 🔹 Filter
        public int[] SelectedCategoryIds { get; set; }
        public int MaxPrice { get; set; } = 200000;
        public string? SearchQuery { get; set; }

    }
}
