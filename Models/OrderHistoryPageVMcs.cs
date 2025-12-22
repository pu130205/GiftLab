namespace GiftLab.ViewModels
{
    public class OrderHistoryPageVM
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<OrderHistoryCardVM> Orders { get; set; } = new();
    }

    public class OrderHistoryCardVM
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string StatusText { get; set; } = "";
        public int StatusId { get; set; }

        public int ItemCount { get; set; }
        public int TotalAmount { get; set; }

        public string Thumb { get; set; } = "";
        public string ProductNames { get; set; } = "";
    }
}
