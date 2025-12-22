using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

[Table("TransacStatus")]
public partial class TransacStatus
{
    [Key]
    public int TransactStatusID { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public string? Description { get; set; }

    [InverseProperty("TransactionStatus")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
