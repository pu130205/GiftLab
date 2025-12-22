using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

public partial class Attribute
{
    [Key]
    public int AttributeID { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    [InverseProperty("Attribute")]
    public virtual ICollection<AttributesPrice> AttributesPrices { get; set; } = new List<AttributesPrice>();
}
