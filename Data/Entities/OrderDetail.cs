using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftLab.Data.Entities;

public partial class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ✅ Identity
    public int OrderDetailID { get; set; }

    public int? OrderID { get; set; }

    public int? ProductID { get; set; }

    public int? OrderNumber { get; set; }

    public int? Quantity { get; set; }

    public int? Discount { get; set; }

    public int? Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ShipDate { get; set; }

    // snapshot item
    public int? UnitPrice { get; set; }

    [StringLength(255)]
    public string? ProductName { get; set; }

    [ForeignKey("OrderID")]
    [InverseProperty("OrderDetails")]
    public virtual Order? Order { get; set; }
}
