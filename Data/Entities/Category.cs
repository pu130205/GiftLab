using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GiftLab.Data.Entities;

public partial class Category
{
    [Key]
    public int CatID { get; set; }

    [StringLength(250)]
    public string? Catname { get; set; }

    public string? Description { get; set; }

    public int? ParentID { get; set; }

    public int? Levels { get; set; }

    public int? Ordering { get; set; }

    public bool Published { get; set; }

    [StringLength(250)]
    public string? Thumb { get; set; }

    [StringLength(250)]
    public string? Title { get; set; }

    [StringLength(250)]
    public string? Alias { get; set; }

    [StringLength(250)]
    public string? MetaDesc { get; set; }

    [StringLength(250)]
    public string? MetaKey { get; set; }

    [StringLength(250)]
    public string? Cover { get; set; }

    public string? SchemaMarkup { get; set; }

    [InverseProperty("Cat")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
