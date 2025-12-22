using GiftLab.Models;
using System.Collections.Generic;

namespace GiftLab.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<ProductCardViewModel> BestSellerProducts { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
