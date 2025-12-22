using System;

namespace GiftLab.ViewModels
{
    public class OrderDetailItemVM
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string Thumb { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
