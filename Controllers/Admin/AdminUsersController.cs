using GiftLab.Data;
using GiftLab.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Controllers.Admin
{
    [Area("Admin")]
    public class AdminUsersController : Controller
    {
        private readonly GiftLabDbContext _db;

        public AdminUsersController(GiftLabDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var users = _db.Accounts
                .AsNoTracking()
                .Include(a => a.Role)
                .OrderByDescending(a => a.CreateDate)
                .Select(a => new AdminUserViewModel
                {
                    AccountID = a.AccountID,
                    FullName = a.Fullname ?? "",
                    Email = a.Email ?? "",
                    Phone = a.Phone ?? "",
                    RoleName = a.Role != null ? (a.Role.RoleName ?? "") : "",
                    Active = a.Active,
                    CreateDate = a.CreateDate
                })
                .ToList();

            return View("~/Views/Admin/Users.cshtml", users);
        }
    }
}
