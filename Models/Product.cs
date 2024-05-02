namespace Backend.Models;

public class Product
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public float Volume { get; set; }
    
    public float Price { get; set; }
    
    // Foreign keys
    
    public long MarketId { get; set; }
    public Market Market { get; set; }
    
    public ICollection<CartItem> CartItems { get; set; }
}