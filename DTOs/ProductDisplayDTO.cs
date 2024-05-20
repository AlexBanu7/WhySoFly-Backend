using Backend.Models;

namespace Backend.DTOs;

public class ProductDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public float Volume { get; set; }
    
    public float PricePerQuantity { get; set; }
    
    public float VolumePerQuantity { get; set; }
    
    public bool SoldByWeight { get; set; }
    
    public byte[] Image { get; set; }
    
    // Foreign keys
    
    public long MarketId { get; set; }
    
    public long CategoryId { get; set; }

    public long NutritionalValuesId { get; set; }

    public static ProductDisplayDTO ToDTO(Product product)
    {
        return new ProductDisplayDTO
        {
            Id = product.Id,
            Name = product.Name,
            PricePerQuantity = product.PricePerQuantity,
            VolumePerQuantity = product.VolumePerQuantity,
            SoldByWeight = product.SoldByWeight,
            Image = product.Image,
            MarketId = product.MarketId,
            CategoryId = product.CategoryId,
            NutritionalValuesId = product.NutritionalValuesId
        };
    }
}