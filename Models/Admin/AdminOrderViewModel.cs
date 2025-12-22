namespace GiftLab.Models.Admin
{
    public class AdminOrderViewModel
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; } = "";
        public int TotalItems { get; set; }
        public int TotalAmount { get; set; }
        public string Status { get; set; } = "";
        public DateTime? OrderDate { get; set; }
    }

    public class AdminOrderDetailItemVM
    {
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int Total => Quantity * UnitPrice;
    }

    public class AdminOrderDetailVM
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime? OrderDate { get; set; }
        public int ShippingFee { get; set; }
        public int GrandTotal { get; set; }
        public string Status { get; set; } = "";
        public List<AdminOrderDetailItemVM> Items { get; set; } = new();
        public int SubTotal { get; set; }
    }
}
