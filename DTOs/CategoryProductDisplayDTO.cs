using Backend.Models;

namespace Backend.DTOs;

public class CategoryProductDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public float PricePerQuantity { get; set; }
    
    public float VolumePerQuantity { get; set; }
    
    public bool SoldByWeight { get; set; }
    
    public long? MarketId { get; set; }
    
    public long? CategoryId { get; set; }
    
    public static CategoryProductDisplayDTO ToDTO(Product product)
    {
        return new CategoryProductDisplayDTO
        {
            Id = product.Id,
            Name = product.Name,
            PricePerQuantity = product.PricePerQuantity,
            VolumePerQuantity = product.VolumePerQuantity,
            SoldByWeight = product.SoldByWeight,
            MarketId = product.MarketId,
            CategoryId = product.CategoryId
        };
    }
}