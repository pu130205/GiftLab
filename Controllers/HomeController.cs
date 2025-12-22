using Microsoft.AspNetCore.Mvc;
using GiftLab.Models;
using GiftLab.ViewModels;
using System.Collections.Generic;
using GiftLab.Data;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly GiftLabDbContext _context;

        public HomeController(GiftLabDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // ============================
            // BEST SELLER PRODUCTS
            // ============================
            var bestSellers = _context.Products
                .Where(p => p.Active)
                .Include(p => p.Cat)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.ProductID,
                    Name = p.ProductName,
                    Category = p.Cat != null ? p.Cat.Catname! : "",
                    ImagePath = string.IsNullOrEmpty(p.Thumb)
                        ? "~/images/default.png"
                        : "" + p.Thumb,
                    Price = p.Price ?? 0,
                    OriginalPrice = p.Discount != null
                        ? p.Price + p.Discount
                        : null,
                    Rating = 5.0, // tạm, sau này có bảng Review
                    SoldCount = _context.OrderDetails
                        .Where(od => od.ProductID == p.ProductID)
                        .Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(p => p.SoldCount)
                .Take(10)
                .ToList();

            var reviews = new List<Review>
                {
                    new Review {
                        Name="Khả Hân", Stars=5,
                        Content="Bó hoa len mini quá dễ thương, cầm trên tay mới thấy tỉ mỉ và tinh tế. Hàng thủ công nhưng hoàn thiện rất đẹp. Giao nhanh, shop tư vấn nhiệt tình. Nhất định sẽ ủng hộ thêm nè 🥰"
                    },
                    new Review {
                        Name="Thanh Vy", Stars=5,
                        Content="Mình đặt một chiếc móc khóa đất sét ở GiftLab để tặng bạn thân, nhận hàng mà mê luôn! Món quà nhỏ nhưng được gói rất cẩn thận, chi tiết tinh tế. Cảm giác đúng kiểu “small but sweet” luôn đó 💗"
                    },
                    new Review {
                        Name="Minh Ngọc", Stars=5,
                        Content="Cookie của GiftLab ngon và xinh hết nấc! Mình đặt để tặng sinh nhật bạn, ai cũng khen dễ thương và ngon nữa. Giao hàng nhanh, hộp quà trang trí rất xịn 🥰"
                    },
                    new Review {
                        Name="Bảo Trân", Stars=5,
                        Content="Mình rất thích phong cách của GiftLab, mọi thứ đều nhẹ nhàng và dễ thương. Sản phẩm nhỏ xinh nhưng làm rất có tâm, nhìn vào là thấy liền sự tỉ mỉ. Rất phù hợp để làm quà tặng 💕"
                    },
                    new Review {
                        Name="Ngọc Mai", Stars=5,
                        Content="Lần đầu mua GiftLab mà ưng ghê luôn. Sản phẩm thực tế xinh hơn hình, màu sắc nhẹ nhàng và nhìn rất có hồn. Nhận quà mà thấy vui hẳn cả ngày 🥰"
                    },
                    new Review {
                        Name="Thu Hà", Stars=5,
                        Content="Mình đã đặt quà GiftLab vài lần rồi và lần nào cũng hài lòng. Đóng gói kỹ, giao nhanh, sản phẩm làm rất chỉnh chu. Nhìn là thấy có sự chăm chút trong từng chi tiết 💗"
                    },
                    new Review {
                        Name="Yến Nhi", Stars=5,
                        Content="Quà GiftLab không quá cầu kỳ nhưng rất tinh tế. Cảm giác mỗi món đều có câu chuyện riêng, cầm lên là thấy ấm áp liền. Rất hợp để tặng người mình thương ✨"
                    },
                    new Review {
                        Name="Phương Anh", Stars=5,
                        Content="Mình mua GiftLab để tặng sinh nhật cho chị gái, chị nhận được là cười hoài luôn. Từ hộp quà tới sản phẩm đều rất xinh và gọn gàng. Nhìn là thấy làm rất có tâm 💝"
                    },
                    new Review {
                        Name="Hồng Nhung", Stars=5,
                        Content="GiftLab mang lại cảm giác rất khác so với mấy shop quà tặng khác. Nhẹ nhàng, dễ thương vừa đủ và rất tinh tế. Đúng kiểu quà nhỏ nhưng làm người nhận vui liền 🫶"
                    },
                };

            var vm = new HomeIndexViewModel
            {
                BestSellerProducts = bestSellers,
                Reviews = reviews
            };

            return View(vm);
        }
    }
}
