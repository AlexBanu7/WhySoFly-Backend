namespace Backend.DTOs;

public class ProductUpdateDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public float PricePerQuantity { get; set; }
    
    public float VolumePerQuantity { get; set; }
    
    public bool SoldByWeight { get; set; }
    
    public byte[]? Image { get; set; }
    
    public long CategoryId { get; set; }
} 