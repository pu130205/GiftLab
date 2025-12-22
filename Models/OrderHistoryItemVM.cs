namespace GiftLab.ViewModels
{
    public class OrderHistoryItemVM
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = "";
        public DateTime? OrderDate { get; set; }
        public int ItemCount { get; set; }
        public int TotalAmount { get; set; }
        public string StatusText { get; set; } = "";
    }
}
