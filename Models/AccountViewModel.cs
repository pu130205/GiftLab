using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GiftLab.Models // Sử dụng namespace chính xác của dự án bạn
{
    // Dữ liệu cho trang Đăng nhập
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    // Dữ liệu cho trang Đăng ký (6 trường chi tiết)
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Họ")]
        public string Ho { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Tên")]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; } // Dùng DateTime? cho trường hợp không nhập

        [Required(ErrorMessage = "Vui lòng chọn Giới tính")]
        public string GioiTinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }
    }

    // ViewModel cho trang Quên mật khẩu (Nếu cần phức tạp hơn, tạm thời dùng string trong Controller)
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }
}