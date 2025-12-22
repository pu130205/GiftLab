using System;
using System.Collections.Generic;

namespace GiftLab.ViewModels
{
    public class OrderDetailPageVM
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }

        public int StatusId { get; set; }
        public string StatusText { get; set; } = "";

        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";

        public string ReceiverName { get; set; } = "";
        public string ReceiverPhone { get; set; } = "";
        public string ReceiverAddress { get; set; } = "";

        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }

        public List<OrderDetailItemVM> Items { get; set; } = new();
    }
}
