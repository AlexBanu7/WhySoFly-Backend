namespace Backend.Models;

public class Product
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public float PricePerQuantity { get; set; }
    
    public float VolumePerQuantity { get; set; }
    
    public bool SoldByWeight { get; set; }
    
    public byte[]? Image { get; set; }
    
    // Foreign keys
    
    public long MarketId { get; set; }
    public Market Market { get; set; }
    
    public long CategoryId { get; set; }
    public Category Category { get; set; }
    
    public long NutritionalValuesId { get; set; }
    public NutritionalValues NutritionalValues { get; set; }
    
    public ICollection<CartItem> CartItems { get; set; }
} 