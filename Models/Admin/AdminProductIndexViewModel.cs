using System.Collections.Generic;

namespace GiftLab.ViewModels.Admin
{
    public class AdminProductIndexViewModel
    {
        public List<AdminProductViewModel> Products { get; set; } = new();
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalCategories { get; set; }
    }
}
