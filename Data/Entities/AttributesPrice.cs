using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

public partial class AttributesPrice
{
    [Key]
    public int AttributesPriceID { get; set; }

    public int? AttributeID { get; set; }

    public int? ProductID { get; set; }

    public int? Price { get; set; }

    public bool Active { get; set; }

    [ForeignKey("AttributeID")]
    [InverseProperty("AttributesPrices")]
    public virtual Attribute? Attribute { get; set; }

    [ForeignKey("ProductID")]
    [InverseProperty("AttributesPrices")]
    public virtual Product? Product { get; set; }
}
