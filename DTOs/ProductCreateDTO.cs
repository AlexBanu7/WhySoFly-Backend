using Backend.Models;

namespace Backend.DTOs;

public class ProductCreateDTO
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public float PricePerQuantity { get; set; }
    
    public float VolumePerQuantity { get; set; }
    
    public bool SoldByWeight { get; set; }
    
    public string? Image { get; set; }
    
    public long MarketId { get; set; }
    
    public NutritionalValuesCreateDTO NutritionalValues { get; set; }
    
    public long CategoryId { get; set; }
}