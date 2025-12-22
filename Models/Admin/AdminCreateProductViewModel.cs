using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GiftLab.ViewModels.Admin
{
    public class AdminCreateProductViewModel
    {
        [Required]
        public string ProductName { get; set; } = null!;

        public string? ShortDesc { get; set; }

        public string? Description { get; set; }

        public int? Price { get; set; }

        public int? UnitsInStock { get; set; }

        public int? CatID { get; set; }

        public bool Active { get; set; } = true;

        public IFormFile? Image { get; set; }
    }
}
