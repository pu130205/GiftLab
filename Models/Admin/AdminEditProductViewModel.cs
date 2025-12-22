using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GiftLab.ViewModels.Admin
{
    public class AdminEditProductViewModel
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public string ProductName { get; set; } = null!;

        public string? ShortDesc { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int UnitsInStock { get; set; }

        [Required]
        public int CatID { get; set; }

        public bool Active { get; set; }

        public IFormFile? Image { get; set; }

        // ảnh cũ
        public string? CurrentImage { get; set; }
    }
}
