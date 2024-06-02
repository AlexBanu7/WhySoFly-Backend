namespace Backend.Models;

public class CartItem
{
    public long Id { get; set; }
    
    public bool Accepted { get; set; }
    
    public string Name { get; set; }
    
    public double Quantity { get; set; }
    
    public double Volume { get; set; }
    
    public double Price { get; set; }
    
    public byte[]? Image { get; set; }
    
    public bool Removed { get; set; }
    
    // Foreign Keys
    
    public long CartId { get; set; }
    public Cart Cart { get; set; }
    
    public long ProductId { get; set; }
    public Product Product { get; set; }
}