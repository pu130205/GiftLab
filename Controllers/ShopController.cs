using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GiftLab.Data;
using GiftLab.ViewModels;
using System;
//using System.Linq;
using GiftLab.Models;
using System.Linq;


namespace GiftLab.Controllers
{
    public class ShopController : Controller
    {
        private readonly GiftLabDbContext _context;

        public ShopController(GiftLabDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // 📌 DANH SÁCH SẢN PHẨM
        // =========================================================
        public IActionResult Index(
            string? q = null,              // ✅ từ khóa tìm theo tên
            int page = 1,
            int? maxPrice = null,
            int[]? catIds = null
        )

        {
            const int pageSize = 8;

             // ===============================
             // QUERY GỐC
             // ===============================
             var query = _context.Products
                 .AsNoTracking()
                 .Include(p => p.Cat)
                 .Where(p => p.Active);
            // ===============================
            // 🔹 TÌM KIẾM THEO TÊN SẢN PHẨM
            // ===============================
            if (!string.IsNullOrWhiteSpace(q))
            {
                var key = q.Trim();
                query = query.Where(p => p.ProductName != null
                    && EF.Functions.Like(p.ProductName, $"%{key}%"));
            }

            // ===============================
            // 🔹 LỌC THEO DANH MỤC
            // ===============================
            if (catIds != null && catIds.Length > 0)
             {
                 query = query.Where(p =>
                     p.CatID.HasValue &&
                     catIds.Any(id => id == p.CatID.Value)
                 );
             }


             // ===============================
             // 🔹 LỌC THEO GIÁ
             // ===============================
             if (maxPrice.HasValue)
             {
                 query = query.Where(p => (p.Price ?? 0) <= maxPrice.Value);
             }

             // ===============================
             // 🔹 PHÂN TRANG
             // ===============================
             var total = query.Count();
             var totalPages = (int)Math.Ceiling(total / (double)pageSize);
             page = Math.Clamp(page, 1, Math.Max(1, totalPages));

             // ===============================
             // 🔹 DANH SÁCH SẢN PHẨM
             // ===============================
             var products = query
                 .OrderByDescending(p => p.ProductID)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .Select(p => new ProductCardViewModel
                 {
                     Id = p.ProductID,
                     Name = p.ProductName,
                     Category = p.Cat != null ? p.Cat.Catname : "",
                     ImagePath = p.Thumb,
                     Price = p.Price ?? 0,
                     OriginalPrice = (p.Discount != null && p.Discount > 0)
                         ? p.Price + p.Discount
                         : null,
                     Rating = 5
                 })
                 .ToList();

             // ===============================
             // 🔹 DANH MỤC (ĐỂ HIỂN THỊ FILTER)
             // ===============================
             var categories = _context.Categories
                 .AsNoTracking()
                 .Where(c => c.Published)
                 .Select(c => new CategoryViewModel
                 {
                     Id = c.CatID,
                     Name = c.Catname
                 })
                 .ToList();

            // ===============================
            // 🔹 VIEWMODEL
            // ===============================
            var vm = new ShopIndexViewModel
            {
                Products = products,
                Categories = categories,
                SelectedCategoryIds = catIds ?? Array.Empty<int>(),
                MaxPrice = maxPrice ?? 200000,
                Page = page,
                TotalPages = totalPages,
                SearchQuery = q
            };

            return View(vm);
         }

         // =========================================================
         // 📌 CHI TIẾT SẢN PHẨM
         // =========================================================
         [HttpGet]
         public IActionResult Details(int id, ProductVariant v)
         {
             var product = _context.Products
                 .Include(p => p.Cat)
                 .Include(p => p.AttributesPrices)
                     .ThenInclude(ap => ap.Attribute)
                 .FirstOrDefault(p => p.ProductID == id && p.Active);

             if (product == null)
                 return NotFound();

             // -----------------------------
             // 🔹 BIẾN THỂ
             // -----------------------------
             var variants = product.AttributesPrices
                 .Where(v => v.Active)
                 .Select(v => new ProductVariant
                 {
                     Id = v.AttributesPriceID,
                     Name = v.Attribute != null ? v.Attribute.Name : "Mặc định",
                     Price = v.Price ?? product.Price ?? 0,
                     OriginalPrice = null,
                     ImagePath = "" + product.Thumb,
                     StockQuantity = product.UnitsInStock ?? 0
                 })
                 .ToList();

             if (!variants.Any())
             {
                 variants.Add(new ProductVariant
                 {
                     Id = 0, // ✅ 0 nghĩa là "không có variant"
                     Name = "Mặc định",
                     Price = product.Price ?? 0,
                     OriginalPrice = null,
                     ImagePath = product.Thumb ?? "/images/default.png",
                     StockQuantity = product.UnitsInStock ?? 0,
                     StockStatus = (product.UnitsInStock ?? 0) > 0 ? "Còn hàng" : "Hết hàng"
                 });
             }
             else
             {
                 // ✅ nếu có biến thể thật thì nên set stock status theo biến thể nếu bạn có cột stock riêng
                 // tạm: dùng stock của product
                 variants = variants.Select(v =>
                 {
                     v.StockStatus = (product.UnitsInStock ?? 0) > 0 ? "Còn hàng" : "Hết hàng";
                     return v;
                 }).ToList();
             }

             // -----------------------------
             // 🔹 SẢN PHẨM LIÊN QUAN
             // -----------------------------
             var relatedProducts = _context.Products
                 .AsNoTracking()
                 .Where(p => p.CatID == product.CatID && p.ProductID != product.ProductID && p.Active)
                 .Take(4)
                 .Select(p => new ProductCardViewModel
                 {
                     Id = p.ProductID,
                     Name = p.ProductName,
                     Category = p.Cat != null ? p.Cat.Catname : "",
                     ImagePath = "" + p.Thumb,
                     Price = p.Price ?? 0,
                     OriginalPrice = null,
                     Rating = 5
                 })
                 .ToList();

             // -----------------------------
             // 🔹 THÔNG TIN SẢN PHẨM (VIEWMODEL)
             // -----------------------------
             var productInfo = new ProductViewModel
             {
                 Id = product.ProductID,
                 Name = product.ProductName,
                 Category = product.Cat?.Catname,
                 Price = product.Price ?? 0,
                 ImagePath = "" + product.Thumb,
                 ShortDescription = product.ShortDesc,
                 Description = product.Description
             };

             var vm = new ProductDetailViewModel
             {
                 ProductInfo = productInfo,
                 ShortDescription = product.ShortDesc,
                 Variants = variants,
                 Breadcrumb = $"Trang Chủ / Cửa Hàng / {product.Cat?.Catname}",
                 DefaultVariantId = variants.FirstOrDefault()?.Id ?? 0,
                 IsFavorite = false,
                 RelatedProducts = relatedProducts
             };

             return View(vm);
         }
    }
}
