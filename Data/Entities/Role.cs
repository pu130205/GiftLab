using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

public partial class Role
{
    [Key]
    public int RoleID { get; set; }

    [StringLength(50)]
    public string? RoleName { get; set; }

    [StringLength(50)]
    public string? Description { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
