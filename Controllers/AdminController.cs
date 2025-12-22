using GiftLab.Data;
using GiftLab.Data.Entities;
using GiftLab.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GiftLab.Controllers
{
    public class AdminController : Controller
    {
        private readonly GiftLabDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(GiftLabDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Route("admin")]
        [Route("admin/index")]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        [Route("admin/dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard - Admin";
            return View();
        }

        [Route("admin/analytics")]
        public IActionResult Analytics()
        {
            ViewData["Title"] = "Analytics - Admin";
            return View();
        }

        [Route("admin/users")]
        public IActionResult Users()
        {
            ViewData["Title"] = "Users - Admin";
            return View();
        }

        [Route("admin/products")]
        public async Task<IActionResult> Products(
            string? category,
            string? status,
            decimal? minPrice,
            decimal? maxPrice,
            int? minStock,
            int? maxStock,
            string? q
        )
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Cat)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim().ToLower();
                query = query.Where(p => p.Cat != null && p.Cat.Catname.ToLower() == cat);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "In Stock")
                    query = query.Where(p => (p.UnitsInStock ?? 0) > 0);
                else if (status == "Out of Stock")
                    query = query.Where(p => (p.UnitsInStock ?? 0) <= 0);
                else if (status == "Low Stock")
                    query = query.Where(p => (p.UnitsInStock ?? 0) > 0 && (p.UnitsInStock ?? 0) <= 5);
            }

            if (minPrice.HasValue)
                query = query.Where(p => (p.Price ?? 0) >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => (p.Price ?? 0) <= maxPrice.Value);

            if (minStock.HasValue)
                query = query.Where(p => (p.UnitsInStock ?? 0) >= minStock.Value);

            if (maxStock.HasValue)
                query = query.Where(p => (p.UnitsInStock ?? 0) <= maxStock.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim();
                query = query.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    (p.ShortDesc != null && p.ShortDesc.Contains(keyword)) ||
                    (p.Description != null && p.Description.Contains(keyword))
                );
            }

            var products = await query
                .Select(p => new AdminProductViewModel
                {
                    Id = p.ProductID,
                    Name = p.ProductName,
                    Category = p.Cat != null ? p.Cat.Catname : "",
                    Price = p.Price ?? 0,
                    Stock = p.UnitsInStock ?? 0,
                    Active = p.Active,
                    Description = p.Description,
                    ShortDesc = p.ShortDesc,
                    ImagePath = p.Thumb != null ? p.Thumb.Replace("~", "") : "/images/no-image.png"
                })
                .ToListAsync();

            var vm = new AdminProductIndexViewModel
            {
                Products = products,
                TotalProducts = products.Count,
                ActiveProducts = products.Count(p => p.Active),
                OutOfStockProducts = products.Count(p => p.Stock <= 0),
                TotalCategories = await _context.Categories.AsNoTracking().CountAsync()
            };

            ViewBag.Filter = new
            {
                category,
                status,
                minPrice,
                maxPrice,
                minStock,
                maxStock,
                q
            };


            ViewData["Title"] = "Products - Admin";
            return View(vm);
        }

        [HttpGet]
        [Route("admin/products/export")]
        public async Task<IActionResult> ExportProducts(
    string? category,
    string? status,
    decimal? minPrice,
    decimal? maxPrice,
    int? minStock,
    int? maxStock,
    string? q
)
        {
            // 1) Query giống Products() để xuất đúng dữ liệu đang lọc
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Cat)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim().ToLower();
                query = query.Where(p => p.Cat != null && p.Cat.Catname.ToLower() == cat);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "In Stock")
                    query = query.Where(p => (p.UnitsInStock ?? 0) > 0);
                else if (status == "Out of Stock")
                    query = query.Where(p => (p.UnitsInStock ?? 0) <= 0);
                else if (status == "Low Stock")
                    query = query.Where(p => (p.UnitsInStock ?? 0) > 0 && (p.UnitsInStock ?? 0) <= 5);
            }

            if (minPrice.HasValue)
                query = query.Where(p => (p.Price ?? 0) >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => (p.Price ?? 0) <= maxPrice.Value);

            if (minStock.HasValue)
                query = query.Where(p => (p.UnitsInStock ?? 0) >= minStock.Value);

            if (maxStock.HasValue)
                query = query.Where(p => (p.UnitsInStock ?? 0) <= maxStock.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim();
                query = query.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    (p.ShortDesc != null && p.ShortDesc.Contains(keyword)) ||
                    (p.Description != null && p.Description.Contains(keyword))
                );
            }

            var products = await query.ToListAsync();

            // 2) Xuất CSV (giống kiểu Orders)
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ID,Tên sản phẩm,Danh mục,Giá,Tồn kho,Trạng thái");

            foreach (var p in products)
            {
                var catName = p.Cat?.Catname ?? "";
                var price = p.Price ?? 0;
                var stock = p.UnitsInStock ?? 0;
                var statusText = stock > 0 ? "Còn hàng" : "Hết hàng";

                // escape dấu " trong csv
                string esc(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";

                sb.AppendLine($"{p.ProductID},{esc(p.ProductName)},{esc(catName)},{price},{stock},{esc(statusText)}");
            }

            // 3) Trả file
            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"products_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }


        [HttpPost]
        [Route("admin/products/create")]
        public async Task<IActionResult> CreateProduct(AdminCreateProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("D? li?u không h?p l?");

            string? imagePath = null;

            if (model.Image != null && model.Image.Length > 0)
            {
                var imagesDir = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(imagesDir);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                var savePath = Path.Combine(imagesDir, fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                imagePath = "/images/" + fileName;
            }

            var product = new Product
            {
                ProductName = model.ProductName,
                ShortDesc = model.ShortDesc,
                Description = model.Description,
                Price = model.Price,
                UnitsInStock = model.UnitsInStock,
                CatID = model.CatID,
                Active = model.Active,
                Thumb = imagePath,
                BestSellers = false,
                HomeFLag = false,
                DateCreated = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [Route("admin/orders")]
        public IActionResult Orders()
        {
            ViewData["Title"] = "Orders - Admin";
            return View();
        }

        [Route("admin/settings")]
        public IActionResult Settings()
        {
            ViewData["Title"] = "Settings - Admin";
            return View();
        }

        [HttpPost]
        [Route("admin/products/edit")]
        public async Task<IActionResult> EditProduct([FromForm] AdminEditProductViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

            var product = await _context.Products.FindAsync(model.ProductID);
            if (product == null)
                return NotFound(new { success = false, message = "Không tìm thấy sản phẩm" });

            try
            {
                product.ProductName = model.ProductName;
                product.ShortDesc = model.ShortDesc;
                product.Description = model.Description;
                product.Price = model.Price;
                product.UnitsInStock = model.UnitsInStock;
                product.CatID = model.CatID;
                product.Active = model.Active;
                product.DateModified = DateTime.Now;

                if (model.Image != null && model.Image.Length > 0)
                {
                    // ✅ đảm bảo folder tồn tại: wwwroot/images
                    var imagesDir = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(imagesDir);

                    var fileName = Guid.NewGuid() + Path.GetExtension(model.Image.FileName);
                    var savePath = Path.Combine(imagesDir, fileName);

                    using var stream = new FileStream(savePath, FileMode.Create);
                    await model.Image.CopyToAsync(stream);

                    product.Thumb = "/images/" + fileName;
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("admin/products/delete")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound("Không t?m th?y s?n ph?m");

            // Xóa ?nh trong wwwroot/images
            if (!string.IsNullOrEmpty(product.Thumb))
            {
                var imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    product.Thumb.TrimStart('/')
                );

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


    }
}

