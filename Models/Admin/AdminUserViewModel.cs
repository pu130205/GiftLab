using System;

namespace GiftLab.ViewModels.Admin
{
    public class AdminUserViewModel
    {
        public int AccountID { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string RoleName { get; set; } = "";
        public bool Active { get; set; }
        public DateTime? CreateDate { get; set; }

        public string Username =>
            !string.IsNullOrWhiteSpace(Email)
                ? ("@" + Email.Split('@')[0])
                : ("@user" + AccountID);
    }
}
