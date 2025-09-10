using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using {{ProjectName}}.Common.Enums;

namespace {{ProjectName}}.Server.Data;

/// <summary>
/// sample entity class representing a product in the database.
/// </summary>
public class Product
{
    [Key, MaxLength(100)]
    public string Id { get; private set; } = null!;

    [MaxLength(100)]
    public string? ParentId { get; set; }

    [StringLength(450)]
    public string Name { get; set; } = null!;

    [StringLength(450)]
    public string? ImageUrl { get; set; }

    [StringLength(4000)]
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public ProductType ProductType { get; set; }

    public ProductStatus ProductStatus { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = null!;

    public Product()
    {
        Id = Guid.NewGuid().ToString();
    }
}