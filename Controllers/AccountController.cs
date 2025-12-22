using GiftLab.Data;
using GiftLab.Data.Entities;
using GiftLab.Models;
using GiftLab.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GiftLab.Controllers
{
    public class AccountController : Controller
    {
        private readonly GiftLabDbContext _db;

        public AccountController(GiftLabDbContext db) => _db = db;

        private int? GetCurrentCustomerId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : null;
        }

        private static string HashPassword(string raw)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw ?? ""));
            return Convert.ToHexString(bytes);
        }

        // ========================= AUTH =========================

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = (model.Email ?? "").Trim();
            var passHash = HashPassword(model.Password ?? "");

            // A) Customer
            var customer = await _db.Customers.FirstOrDefaultAsync(x =>
                x.Email == email && x.Active == true && x.Password == passHash);

            if (customer != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, customer.CustomerID.ToString()),
                    new Claim(ClaimTypes.Name, customer.FullName ?? customer.Email ?? ""),
                    new Claim(ClaimTypes.Email, customer.Email ?? ""),
                    new Claim(ClaimTypes.Role, "Customer"),
                    new Claim("UserType", "Customer"),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

                TempData["ToastSuccess"] = "Đăng nhập thành công 🎉";
                return RedirectToAction("Index", "Home");
            }

            // B) Admin/Staff
            var acc = await _db.Accounts
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email && x.Active && x.Password == passHash);

            if (acc != null)
            {
                var roleName = acc.Role?.RoleName ?? "Admin";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, acc.AccountID.ToString()),
                    new Claim(ClaimTypes.Name, acc.Fullname ?? acc.Email ?? ""),
                    new Claim(ClaimTypes.Email, acc.Email ?? ""),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("UserType", "Account"),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

                TempData["ToastSuccess"] = "Đăng nhập thành công 🎉";
                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            return View(model);
        }
        // ========================= FORGOT PASSWORD =========================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = (model.Email ?? "").Trim();

            var customer = await _db.Customers
                .FirstOrDefaultAsync(x => x.Email == email && x.Active == true);


            ViewBag.Message = "Nếu email tồn tại, GiftLab đã gửi hướng dẫn đặt lại mật khẩu 💌";


            return View(model);
        }


        // ========================= GOOGLE LOGIN =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // Bạn đã xoá Facebook => chỉ cho Google
            if (!string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Provider không hợp lệ.");

            returnUrl ??= Url.Action("Index", "Home");

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var props = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(props, provider); // provider = "Google"
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Action("Index", "Home");

            if (!string.IsNullOrEmpty(remoteError))
            {
                TempData["AuthError"] = $"Đăng nhập Google thất bại: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            // Lấy principal từ external provider (Google)
            var authResult = await HttpContext.AuthenticateAsync();
            var principal = authResult?.Principal;

            if (principal == null)
            {
                TempData["AuthError"] = "Không lấy được thông tin từ Google.";
                return RedirectToAction(nameof(Login));
            }

            var email = principal.FindFirstValue(ClaimTypes.Email);
            var fullName = principal.FindFirstValue(ClaimTypes.Name) ?? "";

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["AuthError"] = "Tài khoản Google không cung cấp email.";
                return RedirectToAction(nameof(Login));
            }

            email = email.Trim();

            // 1) Ưu tiên Customer (web bán hàng)
            var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Email == email);

            if (customer == null)
            {
                // 2) Chưa có => tạo mới
                customer = new Customer
                {
                    FullName = string.IsNullOrWhiteSpace(fullName) ? email : fullName.Trim(),
                    Email = email,
                    Password = "", // login Google => không dùng password
                    Active = true,
                    CreateDate = DateTime.Now,
                };

                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();
            }
            else
            {
                // Bị khoá
                if (customer.Active != true)
                {
                    TempData["AuthError"] = "Tài khoản đã bị vô hiệu hoá.";
                    return RedirectToAction(nameof(Login));
                }
            }

            // 3) Sign-in cookie như login thường
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerID.ToString()),
                new Claim(ClaimTypes.Name, customer.FullName ?? customer.Email ?? ""),
                new Claim(ClaimTypes.Email, customer.Email ?? ""),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("UserType", "Customer"),
                new Claim("LoginProvider", "Google"),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            TempData["ToastSuccess"] = "Đăng nhập Google thành công 🎉";
            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = (model.Email ?? "").Trim();
            var exists = await _db.Customers.AnyAsync(x => x.Email == email);
            if (exists)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(model);
            }

            var customer = new Customer
            {
                FullName = $"{model.Ho} {model.Ten}".Trim(),
                Email = email,
                Password = HashPassword(model.Password ?? ""),
                Active = true,
                CreateDate = DateTime.Now,
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ========================= CUSTOMER AREA =========================

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null) return RedirectToAction(nameof(Login));

            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerID == customerId.Value && (x.Active ?? false));

            if (customer == null) return RedirectToAction(nameof(Login));

            return View(customer);
        }

        // ✅ THÊM LẠI Profile để hết lỗi "Profile does not exist"
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null) return RedirectToAction(nameof(Login));

            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerID == customerId.Value && (x.Active ?? false));

            if (customer == null) return RedirectToAction(nameof(Login));

            var vm = new AccountProfileViewModel
            {
                CustomerID = customer.CustomerID,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                Birthday = customer.Birthday,
                Address = customer.Address,
                LocationID = customer.LocationID,
                District = customer.District,
                Ward = customer.Ward,
                Avatar = customer.Avatar,
                Active = customer.Active,
                CreateDate = customer.CreateDate
            };

            return View(vm);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AccountProfileViewModel model)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null) return RedirectToAction(nameof(Login));

            if (model.CustomerID != customerId.Value) return Forbid();

            var customer = await _db.Customers
                .FirstOrDefaultAsync(x => x.CustomerID == customerId.Value && (x.Active ?? false));

            if (customer == null) return RedirectToAction(nameof(Login));

            customer.FullName = (model.FullName ?? "").Trim();
            customer.Phone = (model.Phone ?? "").Trim();
            customer.Birthday = model.Birthday;
            customer.Address = (model.Address ?? "").Trim();
            customer.LocationID = model.LocationID;
            customer.District = model.District;
            customer.Ward = model.Ward;
            customer.Email = (model.Email ?? "").Trim();

            await _db.SaveChangesAsync();

            TempData["ToastSuccess"] = "Cập nhật thông tin thành công ✅";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new GiftLab.Models.ChangePasswordViewModel());
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(GiftLab.Models.ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customerId = GetCurrentCustomerId();
            if (customerId == null) return RedirectToAction(nameof(Login));

            var customer = await _db.Customers
                .FirstOrDefaultAsync(x => x.CustomerID == customerId.Value && (x.Active ?? false));

            if (customer == null) return RedirectToAction(nameof(Login));

            var currentHash = HashPassword(model.CurrentPassword ?? "");
            if (!string.Equals(customer.Password ?? "", currentHash, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            customer.Password = HashPassword(model.NewPassword ?? "");
            await _db.SaveChangesAsync();

            TempData["ToastSuccess"] = "Đổi mật khẩu thành công ✅";
            return RedirectToAction(nameof(Profile));
        }

        // ========================= ORDER HISTORY =========================

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null) return RedirectToAction(nameof(Login));

            var customer = await _db.Customers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerID == customerId.Value && (x.Active ?? false));
            if (customer == null) return RedirectToAction(nameof(Login));

            var ordersRaw = await _db.Orders
                .AsNoTracking()
                .Where(o => o.CustomerID == customerId.Value && (o.Deleted == null || o.Deleted == false))
                .Include(o => o.TransactionStatus)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var productIds = ordersRaw
                .SelectMany(o => o.OrderDetails.Select(d => d.ProductID))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var productMap = await _db.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.ProductID))
                .Select(p => new { p.ProductID, p.ProductName, p.Thumb })
                .ToDictionaryAsync(x => x.ProductID, x => x);

            var vm = new OrderHistoryPageVM
            {
                FullName = customer.FullName ?? "",
                Email = customer.Email ?? "",
                Orders = ordersRaw.Select(o =>
                {
                    var itemCount = o.OrderDetails.Sum(d => d.Quantity ?? 0);
                    var total = o.OrderDetails.Sum(d => d.Total ?? 0) + (o.ShippingFee ?? 0);

                    var firstProductId = o.OrderDetails.Select(d => d.ProductID).FirstOrDefault();
                    var thumb = (firstProductId.HasValue && productMap.ContainsKey(firstProductId.Value))
                        ? (productMap[firstProductId.Value].Thumb ?? "")
                        : "";

                    var names = o.OrderDetails
                        .Select(d => d.ProductID)
                        .Where(id => id.HasValue && productMap.ContainsKey(id.Value))
                        .Select(id => productMap[id!.Value].ProductName)
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Distinct()
                        .Take(3)
                        .ToList();

                    return new OrderHistoryCardVM
                    {
                        OrderId = o.OrderID,
                        OrderDate = o.OrderDate,
                        StatusId = o.TransactionStatusID ?? 0,
                        StatusText = o.TransactionStatus?.Status ?? "Đang xử lý",
                        ItemCount = itemCount,
                        TotalAmount = total,
                        Thumb = thumb,
                        ProductNames = string.Join(", ", names)
                    };
                }).ToList()
            };

            return View("OrderHistory", vm);
        }

        // ========================= ORDER DETAILS =========================

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null) return RedirectToAction(nameof(Login));

            // chỉ cho xem đơn của chính mình
            var order = await _db.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.TransactionStatus)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == id && o.CustomerID == customerId.Value);

            if (order == null) return NotFound();

            // map ProductID -> Thumb từ Products
            var pids = order.OrderDetails
                .Select(d => d.ProductID)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var thumbMap = await _db.Products
                .AsNoTracking()
                .Where(p => pids.Contains(p.ProductID))
                .Select(p => new { p.ProductID, p.Thumb })
                .ToDictionaryAsync(x => x.ProductID, x => x.Thumb);

            var items = order.OrderDetails
                .OrderBy(d => d.OrderNumber ?? 0)
                .Select(d =>
                {
                    var qty = d.Quantity ?? 0;
                    var unit = (decimal)(d.UnitPrice ?? 0);
                    var line = (decimal)(d.Total ?? (int)(unit * qty));

                    var thumb = "";
                    if (d.ProductID.HasValue && thumbMap.TryGetValue(d.ProductID.Value, out var t))
                        thumb = t ?? "";

                    return new OrderDetailItemVM
                    {
                        ProductId = d.ProductID,
                        ProductName = d.ProductName ?? "",
                        Thumb = thumb, 
                        Quantity = qty,
                        UnitPrice = unit,
                        LineTotal = line
                    };
                })
                .ToList();

            var subtotal = items.Sum(x => x.LineTotal);
            var shippingFee = (decimal)(order.ShippingFee ?? 0);
            var total = subtotal + shippingFee;

            var vm = new OrderDetailPageVM
            {
                OrderId = order.OrderID,
                OrderDate = order.OrderDate,

                StatusId = order.TransactionStatusID ?? 0,
                StatusText = order.TransactionStatus?.Status ?? "",

                FullName = order.Customer?.FullName ?? "",
                Email = order.Customer?.Email ?? "",

                ReceiverName = order.ReceiverName ?? "",
                ReceiverPhone = order.ReceiverPhone ?? "",
                ReceiverAddress = string.Join(", ",
                    new[] { order.ShipAddress, order.ShipWard, order.ShipDistrict }
                        .Where(s => !string.IsNullOrWhiteSpace(s))),

                Items = items,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Total = total
            };

            return View(vm);
        }
    }
}
