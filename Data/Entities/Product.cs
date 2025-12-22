using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

public partial class Product
{
    [Key]
    public int ProductID { get; set; }

    [StringLength(255)]
    public string ProductName { get; set; } = null!;

    [StringLength(255)]
    public string? ShortDesc { get; set; }

    public string? Description { get; set; }

    public int? CatID { get; set; }

    public int? Price { get; set; }

    public int? Discount { get; set; }

    [StringLength(255)]
    public string? Thumb { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    public bool BestSellers { get; set; }

    public bool HomeFLag { get; set; }

    public bool Active { get; set; }

    public string? Tags { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    [StringLength(255)]
    public string? Alias { get; set; }

    [StringLength(255)]
    public string? MetaDesc { get; set; }

    [StringLength(255)]
    public string? MetaKey { get; set; }

    public int? UnitsInStock { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<AttributesPrice> AttributesPrices { get; set; } = new List<AttributesPrice>();

    [ForeignKey("CatID")]
    [InverseProperty("Products")]
    public virtual Category? Cat { get; set; }
}
