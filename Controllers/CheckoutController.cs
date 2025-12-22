using GiftLab.Data;
using GiftLab.Data.Entities;
using GiftLab.Helpers;
using GiftLab.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiftLab.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly GiftLabDbContext _db;

        private const string CART_KEY = "CART";
        private const string BUYNOW_KEY = "BUYNOW";

        public CheckoutController(GiftLabDbContext db) => _db = db;

        // GET: /Checkout
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult Index()
        {
            // ✅ đọc cart đúng type Models
            var cart = HttpContext.Session.GetObject<GiftLab.Models.CartViewModel>(CART_KEY);
            var justOrdered = TempData.ContainsKey("OrderSuccess");

            var vm = new CheckoutViewModel
            {
                Mode = "CART",
                Items = new List<CheckoutItemVM>(),
                PaymentMethod = "COD",
                ShippingFee = 0
            };

            if (cart?.Items != null && cart.Items.Any())
            {
                vm.Items = cart.Items.Select(x => new CheckoutItemVM
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity
                }).ToList();
            }
            else
            {
                if (!justOrdered)
                {
                    // giữ nguyên như bạn đang để trống
                }
            }

            // Prefill info customer từ Claims + DB
            var customerId = GetCurrentCustomerId();
            if (customerId > 0)
            {
                var customer = _db.Customers.AsNoTracking()
                    .FirstOrDefault(x => x.CustomerID == customerId && (x.Active ?? false));

                if (customer != null)
                {
                    vm.FullName = customer.FullName ?? "";
                    vm.Phone = customer.Phone ?? "";
                    vm.Email = customer.Email ?? "";
                    vm.Address = customer.Address ?? "";
                    vm.City = customer.District ?? ""; // bạn đang map City = District
                    vm.Ward = customer.Ward ?? "";
                }
            }

            // ✅ tính ship đúng rule (TPHCM = 22k, còn lại 30k)
            vm.ShippingFee = ShippingHelper.CalcShippingFee(vm.City);

            return View(vm);
        }

        // GET: /Checkout/BuyNow?productId=1&quantity=2&variantId=3
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult BuyNow(int productId, int quantity = 1, int? variantId = null)
        {
            if (quantity < 1) quantity = 1;

            var product = _db.Products
                .AsNoTracking()
                .FirstOrDefault(p => p.ProductID == productId && p.Active);

            if (product == null) return NotFound();

            int unitPrice = product.Price ?? 0;
            string displayName = product.ProductName;
            string imagePath = product.Thumb ?? "";

            if (variantId.HasValue && variantId.Value > 0)
            {
                var ap = _db.AttributesPrices
                    .AsNoTracking()
                    .Include(x => x.Attribute)
                    .FirstOrDefault(x => x.AttributesPriceID == variantId.Value
                                      && x.ProductID == productId
                                      && x.Active);

                if (ap != null)
                {
                    unitPrice = ap.Price ?? unitPrice;
                    var attrName = ap.Attribute?.Name;
                    if (!string.IsNullOrWhiteSpace(attrName))
                        displayName = $"{product.ProductName} - {attrName}";
                }
            }

            var items = new List<CheckoutItemVM>
            {
                new CheckoutItemVM
                {
                    ProductId = productId,
                    VariantId = variantId,
                    Name = displayName,
                    ImagePath = imagePath,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                }
            };

            // ✅ Lưu BuyNow vào session để POST PlaceOrder đọc lại
            HttpContext.Session.SetObject(BUYNOW_KEY, items);

            var vm = new CheckoutViewModel
            {
                Mode = "BUYNOW",
                Items = items,
                PaymentMethod = "COD",
                ShippingFee = 0 // ✅ không hardcode
            };

            // Prefill customer
            var customerId = GetCurrentCustomerId();
            if (customerId > 0)
            {
                var customer = _db.Customers.AsNoTracking()
                    .FirstOrDefault(x => x.CustomerID == customerId && (x.Active ?? false));

                if (customer != null)
                {
                    vm.FullName = customer.FullName ?? "";
                    vm.Phone = customer.Phone ?? "";
                    vm.Email = customer.Email ?? "";
                    vm.Address = customer.Address ?? "";
                    vm.City = customer.District ?? "";
                    vm.Ward = customer.Ward ?? "";
                }
            }

            // ✅ tính ship đúng rule
            vm.ShippingFee = ShippingHelper.CalcShippingFee(vm.City);

            return View("Index", vm);
        }

        // POST: /Checkout/PlaceOrder
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model, string PaymentMethod)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId <= 0)
                return Json(new { ok = false, needLogin = true, message = "Bạn cần đăng nhập." });

            List<CheckoutItemVM> items;

            if ((model.Mode ?? "").ToUpper() == "BUYNOW")
            {
                items = HttpContext.Session.GetObject<List<CheckoutItemVM>>(BUYNOW_KEY) ?? new List<CheckoutItemVM>();
            }
            else
            {
                var cart = HttpContext.Session.GetObject<GiftLab.Models.CartViewModel>(CART_KEY);

                items = cart?.Items?.Select(x => new CheckoutItemVM
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity
                }).ToList() ?? new List<CheckoutItemVM>();

                model.Mode = "CART";
            }

            if (items.Count == 0)
                return Json(new { ok = false, message = "Giỏ hàng trống." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors?.Count > 0)
                    .SelectMany(kv => kv.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                return Json(new { ok = false, message = "Vui lòng kiểm tra lại thông tin.", errors });
            }

            int newOrderId = 0;

            // ✅ server tự tính ship (không cho sửa từ client)
            int shipFee = ShippingHelper.CalcShippingFee(model.City);

            var strategy = _db.Database.CreateExecutionStrategy();

            try
            {
                strategy.Execute(() =>
                {
                    using var tx = _db.Database.BeginTransaction();

                    var variantNote = string.Join(" | ", items
                        .Where(x => x.VariantId.HasValue)
                        .Select(x => $"{x.Name} (VariantId={x.VariantId})"));

                    var order = new Order
                    {
                        CustomerID = customerId,
                        OrderDate = DateTime.Now,
                        Deleted = false,
                        Paid = false,

                        Note = string.IsNullOrWhiteSpace(variantNote)
                            ? model.Note
                            : (string.IsNullOrWhiteSpace(model.Note) ? variantNote : (model.Note + " | " + variantNote)),

                        PaymentID = PaymentMethod == "BANK" ? 2 : 1,
                        PaymentData = null,
                        TransactionStatusID = 1,

                        ReceiverName = model.FullName,
                        ReceiverEmail = model.Email,
                        ReceiverPhone = model.Phone,

                        ShipAddress = model.Address,
                        ShipDistrict = model.City,
                        ShipWard = model.Ward,

                        ShippingFee = shipFee
                    };

                    _db.Orders.Add(order);
                    _db.SaveChanges();

                    var details = items.Select((x, idx) => new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = x.ProductId,
                        OrderNumber = idx + 1,
                        Quantity = x.Quantity,
                        Discount = 0,

                        UnitPrice = (int)Math.Round(x.UnitPrice, 0, MidpointRounding.AwayFromZero),
                        ProductName = x.Name,

                        Total = (int?)Math.Round(x.UnitPrice * x.Quantity, 0, MidpointRounding.AwayFromZero),
                        ShipDate = null
                    }).ToList();

                    _db.OrderDetails.AddRange(details);
                    _db.SaveChanges();

                    tx.Commit();

                    newOrderId = order.OrderID;
                });

                if ((model.Mode ?? "").ToUpper() == "BUYNOW") HttpContext.Session.Remove(BUYNOW_KEY);
                else HttpContext.Session.Remove(CART_KEY);

                return Json(new { ok = true, orderId = newOrderId });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return Json(new { ok = false, message = msg });
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult CalcShipping(string? city)
        {
            var fee = ShippingHelper.CalcShippingFee(city);
            return Json(new { ok = true, shippingFee = fee });
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        private int GetCurrentCustomerId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }
    }
}
