using GiftLab.Data;
using GiftLab.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GiftLab.Controllers.Admin
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly GiftLabDbContext _db;
        public AdminOrdersController(GiftLabDbContext db) => _db = db;

        // GET: /Admin/AdminOrders
        // GET: /Admin/AdminOrders
        public async Task<IActionResult> Index(
            string? keyword,
            int? statusId,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? minTotal,
            decimal? maxTotal,
            int page = 1
        )
        {
            // ✅ DEBUG: xem đang nối DB nào
            string dbName = "";
            try { dbName = _db.Database.GetDbConnection().Database ?? ""; } catch { }

            // ✅ Base query
            var q = _db.Orders
                .AsNoTracking()
                .Where(o => o.Deleted != true)
                .Include(o => o.Customer)
                .Include(o => o.TransactionStatus)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            // ✅ Filter: keyword (mã đơn / tên / email / phone)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();

                // Nếu user nhập số -> cho search theo OrderID luôn
                var isId = int.TryParse(kw, out var orderIdKw);

                q = q.Where(o =>
                    (isId && o.OrderID == orderIdKw)
                    || (o.ReceiverName != null && o.ReceiverName.Contains(kw))
                    || (o.ReceiverEmail != null && o.ReceiverEmail.Contains(kw))
                    || (o.ReceiverPhone != null && o.ReceiverPhone.Contains(kw))
                    || (o.Customer != null && o.Customer.FullName != null && o.Customer.FullName.Contains(kw))
                    || (o.Customer != null && o.Customer.Email != null && o.Customer.Email.Contains(kw))
                    || (o.Customer != null && o.Customer.Phone != null && o.Customer.Phone.Contains(kw))
                );
            }


            // ✅ Filter: statusId
            if (statusId.HasValue && statusId.Value > 0)
                q = q.Where(o => o.TransactionStatusID == statusId.Value);

            // ✅ Filter: fromDate
            if (fromDate.HasValue)
            {
                var fd = fromDate.Value.Date;
                q = q.Where(o => o.OrderDate >= fd);
            }

            // ✅ Filter: toDate (inclusive)
            if (toDate.HasValue)
            {
                var tdExclusive = toDate.Value.Date.AddDays(1);
                q = q.Where(o => o.OrderDate < tdExclusive);
            }

            // ✅ Filter: minTotal/maxTotal (tổng = sum(details.Total) + shipping)
            if (minTotal.HasValue)
            {
                var min = minTotal.Value;
                q = q.Where(o =>
                    (o.OrderDetails.Sum(x => (decimal?)(x.Total ?? 0)) ?? 0m) + (decimal?)(o.ShippingFee ?? 0) >= min
                );
            }

            if (maxTotal.HasValue)
            {
                var max = maxTotal.Value;
                q = q.Where(o =>
                    (o.OrderDetails.Sum(x => (decimal?)(x.Total ?? 0)) ?? 0m) + (decimal?)(o.ShippingFee ?? 0) <= max
                );
            }
            const int PAGE_SIZE = 5;
            var totalItems = await q.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PAGE_SIZE);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            // ✅ Project ra VM
            var orders = await q
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .Select(o => new AdminOrderViewModel
                {
                    OrderID = o.OrderID,
                    CustomerName =
                        o.ReceiverName
                        ?? (o.Customer != null ? o.Customer.FullName : null)
                        ?? "N/A",
                    TotalItems = o.OrderDetails.Sum(x => x.Quantity ?? 0),
                    TotalAmount = o.OrderDetails.Sum(x => x.Total ?? 0) + (o.ShippingFee ?? 0),
                    Status = o.TransactionStatus != null ? (o.TransactionStatus.Status ?? "") : "",
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            // ✅ Status list from DB (đổ cho dropdown filter + change status)
            var statuses = await _db.TransacStatuses
                .AsNoTracking()
                .OrderBy(s => s.TransactStatusID)
                .Select(s => new
                {
                    Id = s.TransactStatusID,
                    Name = s.Status ?? ""
                })
                .ToListAsync();

            ViewBag.Statuses = statuses;
            ViewBag.StatusCount = statuses.Count;
            ViewBag.DbName = dbName;
            // ================= KPI (from DB) =================
            const int STATUS_PENDING = 1;
            const int STATUS_COMPLETED = 4;

            var kpiBase = q; // q đã filter sẵn

            var totalOrders = await kpiBase.CountAsync();
            var pendingOrders = await kpiBase.CountAsync(o => o.TransactionStatusID == STATUS_PENDING);
            var completedOrders = await kpiBase.CountAsync(o => o.TransactionStatusID == STATUS_COMPLETED);

            // ===== Revenue bám filter =====
            // chỉ tính doanh thu cho các đơn hoàn thành trong tập đã filter
            var completedIds = await kpiBase
                .Where(o => o.TransactionStatusID == STATUS_COMPLETED)
                .Select(o => o.OrderID)
                .ToListAsync();

            decimal revenueItems = 0m;
            decimal revenueShipping = 0m;

            if (completedIds.Count > 0)
            {
                revenueItems = await _db.OrderDetails
                    .AsNoTracking()
.Where(d => d.OrderID.HasValue && completedIds.Contains(d.OrderID.Value))
                    .SumAsync(d => (decimal?)(d.Total ?? 0)) ?? 0m;

                revenueShipping = await _db.Orders
                    .AsNoTracking()
                    .Where(o => completedIds.Contains(o.OrderID))
                    .SumAsync(o => (decimal?)(o.ShippingFee ?? 0)) ?? 0m;
            }

            var revenue = revenueItems + revenueShipping;


            ViewBag.KpiTotalOrders = totalOrders;
            ViewBag.KpiPendingOrders = pendingOrders;
            ViewBag.KpiCompletedOrders = completedOrders;
            ViewBag.KpiRevenue = revenue;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = PAGE_SIZE;
            ViewBag.TotalItems = totalItems;

            // ✅ để view & JS biết đang filter gì (giữ lại giá trị)
            ViewBag.CurrentFilter = new
            {
                keyword,
                statusId,
                fromDate = fromDate?.ToString("yyyy-MM-dd"),
                toDate = toDate?.ToString("yyyy-MM-dd"),
                minTotal,
                maxTotal
            };


            return View("~/Views/Admin/Orders.cshtml", orders);
        }

        // GET: /Admin/AdminOrders/Export
        [HttpGet]
        public async Task<IActionResult> Export(
            string? keyword,
            int? statusId,
            DateTime? fromDate,
            DateTime? toDate,
            decimal? minTotal,
            decimal? maxTotal
        )

        {
            var q = _db.Orders
                .AsNoTracking()
                .Where(o => o.Deleted != true)
                .Include(o => o.Customer)
                .Include(o => o.TransactionStatus)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            // ✅ Filter: keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                var isId = int.TryParse(kw, out var orderIdKw);

                q = q.Where(o =>
                    (isId && o.OrderID == orderIdKw)
                    || (o.ReceiverName != null && o.ReceiverName.Contains(kw))
                    || (o.ReceiverEmail != null && o.ReceiverEmail.Contains(kw))
                    || (o.ReceiverPhone != null && o.ReceiverPhone.Contains(kw))
                    || (o.Customer != null && o.Customer.FullName != null && o.Customer.FullName.Contains(kw))
                    || (o.Customer != null && o.Customer.Email != null && o.Customer.Email.Contains(kw))
                    || (o.Customer != null && o.Customer.Phone != null && o.Customer.Phone.Contains(kw))
                );
            }


            // Filter giống Index
            if (statusId.HasValue && statusId.Value > 0)
                q = q.Where(o => o.TransactionStatusID == statusId.Value);

            if (fromDate.HasValue)
                q = q.Where(o => o.OrderDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                q = q.Where(o => o.OrderDate < toDate.Value.Date.AddDays(1));

            if (minTotal.HasValue)
            {
                var min = minTotal.Value;
                q = q.Where(o =>
                    (o.OrderDetails.Sum(x => (decimal?)(x.Total ?? 0)) ?? 0m) + (decimal?)(o.ShippingFee ?? 0) >= min
                );
            }

            if (maxTotal.HasValue)
            {
                var max = maxTotal.Value;
                q = q.Where(o =>
                    (o.OrderDetails.Sum(x => (decimal?)(x.Total ?? 0)) ?? 0m) + (decimal?)(o.ShippingFee ?? 0) <= max
                );
            }

            var orders = await q
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // CSV
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("OrderID,Customer,TotalItems,TotalAmount,Status,OrderDate");

            foreach (var o in orders)
            {
                var customer = o.ReceiverName ?? o.Customer?.FullName ?? "";
                var totalItems = o.OrderDetails.Sum(x => x.Quantity ?? 0);
                var totalAmount = o.OrderDetails.Sum(x => x.Total ?? 0) + (o.ShippingFee ?? 0);
                var status = o.TransactionStatus?.Status ?? "";
                var date = o.OrderDate?.ToString("yyyy-MM-dd") ?? "";

                // escape CSV bằng Replace đơn giản
                customer = customer.Replace("\"", "\"\"");
                status = status.Replace("\"", "\"\"");

                sb.AppendLine($"{o.OrderID},\"{customer}\",{totalItems},{totalAmount},\"{status}\",{date}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"orders_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }


        // GET: /Admin/AdminOrders/Detail?id=123
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var o = await _db.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.TransactionStatus)
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.OrderID == id && x.Deleted != true);

            if (o == null) return Json(new { ok = false, message = "Không tìm thấy đơn." });

            var items = o.OrderDetails
                .OrderBy(x => x.OrderNumber)
                .Select(x => new
                {
                    productName = x.ProductName ?? ("ProductID=" + (x.ProductID ?? 0)),
                    quantity = x.Quantity ?? 0,
                    unitPrice = x.UnitPrice ?? 0,
                    total = x.Total ?? 0
                })
                .ToList();

            var payload = new
            {
                orderId = o.OrderID,
                date = o.OrderDate?.ToString("yyyy-MM-dd HH:mm"),
                status = o.TransactionStatus?.Status ?? "",
                customer = new
                {
                    name = o.ReceiverName ?? o.Customer?.FullName ?? "",
                    email = o.ReceiverEmail ?? o.Customer?.Email ?? "",
                    phone = o.ReceiverPhone ?? o.Customer?.Phone ?? "",
                    address = $"{o.ShipAddress ?? o.Customer?.Address ?? ""}, {o.ShipWard ?? o.Customer?.Ward ?? ""}, {o.ShipDistrict ?? o.Customer?.District ?? ""}"
                },
                items,
                subtotal = o.OrderDetails.Sum(d => (d.Total ?? 0)),
                shippingFee = o.ShippingFee ?? 0,
                grandTotal = o.OrderDetails.Sum(d => (d.Total ?? 0)) + (o.ShippingFee ?? 0)
            };

            return Json(new { ok = true, data = payload });
        }

        // ✅ trả về list thuần để JS dễ parse
        // GET: /Admin/AdminOrders/StatusList
        [HttpGet]
        public async Task<IActionResult> StatusList()
        {
            var list = await _db.TransacStatuses
                .AsNoTracking()
                .OrderBy(x => x.TransactStatusID)
                .Select(x => new
                {
                    id = x.TransactStatusID,
                    name = x.Status ?? ""
                })
                .ToListAsync();

            return Json(list);
        }

        // POST: /Admin/AdminOrders/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, int statusId, string? note)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(x => x.OrderID == orderId && x.Deleted != true);
            if (order == null) return Json(new { ok = false, message = "Không tìm thấy đơn." });

            var st = await _db.TransacStatuses.AsNoTracking().FirstOrDefaultAsync(x => x.TransactStatusID == statusId);
            if (st == null) return Json(new { ok = false, message = "Trạng thái không hợp lệ." });

            order.TransactionStatusID = statusId;

            if (!string.IsNullOrWhiteSpace(note))
                order.Note = string.IsNullOrWhiteSpace(order.Note) ? note : (order.Note + " | " + note);

            await _db.SaveChangesAsync();
            return Json(new { ok = true, status = st.Status ?? "" });
        }
    }
}
