namespace Backend.Models;

public class CartItem
{
    public long Id { get; set; }
    
    public bool Accepted { get; set; }
    
    // Foreign Keys
    
    public long CartId { get; set; }
    public Cart Cart { get; set; }
    
    public long ProductId { get; set; }
    public Product Product { get; set; }
}