using Backend.Models;

namespace Backend.DTOs;

public class ProductDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public float PricePerQuantity { get; set; }
    
    public float VolumePerQuantity { get; set; }
    
    public bool SoldByWeight { get; set; }
    
    public byte[]? Image { get; set; }
    
    // Foreign keys
    
    public static ProductDisplayDTO ToDTO(Product product)
    {
        return new ProductDisplayDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            PricePerQuantity = product.PricePerQuantity,
            VolumePerQuantity = product.VolumePerQuantity,
            SoldByWeight = product.SoldByWeight,
            Image = product.Image,
        };
    }
} 