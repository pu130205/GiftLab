using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

public partial class Account
{
    [Key]
    public int AccountID { get; set; }

    [StringLength(12)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(128)]
    public string? Password { get; set; }


    public bool Active { get; set; }

    [StringLength(150)]
    public string? Fullname { get; set; }

    public int? RoleID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [ForeignKey("RoleID")]
    [InverseProperty("Accounts")]
    public virtual Role? Role { get; set; }
}
