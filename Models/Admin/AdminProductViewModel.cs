namespace GiftLab.ViewModels.Admin
{
    public class AdminProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public int Price { get; set; }
        public int Stock { get; set; }
        public bool Active { get; set; }
        public string ImagePath { get; set; } = "";
        public string? Description { get; internal set; }
        public string? ShortDesc { get; internal set; }
    }
}
