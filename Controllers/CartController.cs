using GiftLab.Data;
using GiftLab.Helpers;
using GiftLab.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GiftLab.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        private const string CART_KEY = "CART";
        private readonly GiftLabDbContext _db;

        public CartController(GiftLabDbContext db) => _db = db;

        // ===================== CART PAGE =====================
        [Authorize(Roles = "Customer")]
        [HttpGet("")]
        public IActionResult Index()
        {
            var cart = GetCart();

            // ✅ tính ship dựa theo info customer (District/City)
            var customerId = GetCurrentCustomerId();
            string? cityForShip = null;

            if (customerId > 0)
            {
                var customer = _db.Customers.AsNoTracking()
                    .FirstOrDefault(x => x.CustomerID == customerId && (x.Active ?? false));

                if (customer != null)
                {
                    // đồng bộ với Checkout: bạn đang dùng District cho City
                    cityForShip = customer.District;
                }
            }

            cart.ShippingFee = ShippingHelper.CalcShippingFee(cityForShip);

            // lưu lại để view dùng ổn định
            SaveCart(cart);

            return View(cart);
        }

        // ===================== ADD =====================
        // ✅ Fallback GET /Cart/Add
        [Authorize]
        [HttpGet("Add")]
        public IActionResult AddGet(int productId, int quantity = 1, int? variantId = null)
        {
            AddToCartCore(productId, quantity, variantId);
            return RedirectToAction(nameof(Index));
        }

        // ✅ POST /Cart/Add
        [Authorize(Roles = "Customer")]
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public IActionResult AddPost(int productId, int quantity = 1, int? variantId = null)
        {
            AddToCartCore(productId, quantity, variantId);
            return RedirectToAction(nameof(Index));
        }

        // ===================== CORE LOGIC =====================
        private void AddToCartCore(int productId, int quantity, int? variantId)
        {
            if (quantity < 1) quantity = 1;

            var product = _db.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null) throw new Exception("Product not found");

            var displayName = product.ProductName;
            var imagePath = product.Thumb ?? "";
            var unitPrice = product.Price ?? 0;

            if (!variantId.HasValue || variantId.Value <= 0) variantId = null;

            if (variantId.HasValue)
            {
                var ap = _db.AttributesPrices
                    .Include(x => x.Attribute)
                    .FirstOrDefault(x => x.AttributesPriceID == variantId.Value
                                      && x.ProductID == productId
                                      && x.Active);

                if (ap == null) throw new Exception("Variant not found");

                unitPrice = ap.Price ?? unitPrice;

                var attrName = ap.Attribute?.Name;
                if (!string.IsNullOrWhiteSpace(attrName))
                    displayName = $"{product.ProductName} - {attrName}";
            }

            var cart = GetCart();

            var existing = cart.Items.FirstOrDefault(x =>
                x.ProductId == productId && x.VariantId == variantId);

            if (existing != null) existing.Quantity += quantity;
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = productId,
                    VariantId = variantId,
                    Name = displayName,
                    ImagePath = imagePath,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
        }

        // ===================== REMOVE / UPDATE =====================
        [Authorize(Roles = "Customer")]
        [HttpPost("Remove")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId, int? variantId = null)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
            if (item != null) cart.Items.Remove(item);

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int productId, int? variantId = null, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
            if (item == null) return RedirectToAction(nameof(Index));

            if (quantity <= 0) cart.Items.Remove(item);
            else item.Quantity = quantity;

            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        // ===================== SESSION HELPERS =====================
        private CartViewModel GetCart()
            => HttpContext.Session.GetObject<CartViewModel>(CART_KEY) ?? new CartViewModel();

        private void SaveCart(CartViewModel cart)
            => HttpContext.Session.SetObject(CART_KEY, cart);

        private int GetCurrentCustomerId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }
    }
}
