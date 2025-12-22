using System;
using System.ComponentModel.DataAnnotations;

namespace GiftLab.Models
{
    public class AccountProfileViewModel
    {
        // Customer
        public int CustomerID { get; set; }

        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Ảnh đại diện (đường dẫn)")]
        public string? Avatar { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [Display(Name = "Tỉnh/Thành phố (LocationID)")]
        public string? LocationID { get; set; }

        [Display(Name = "Quận/Huyện (District)")]
        public string? District { get; set; }

        [Display(Name = "Phường/Xã (Ward)")]
        public string? Ward { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool? Active { get; set; }

        public DateTime? CreateDate { get; set; }

        // Account (nếu bạn muốn hiển thị thêm)
        public int? AccountID { get; set; }
        public int? RoleID { get; set; }
    }
}
