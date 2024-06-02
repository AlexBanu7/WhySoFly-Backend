using Backend.Models;

namespace Backend.DTOs;

public class CartItemCreateDTO
{
    public string Name { get; set; }
    
    public double Quantity { get; set; }
    
    public double Volume { get; set; }
    
    public double Price { get; set; }
    
    public bool Accepted { get; set; }
    
    // Foreign Keys
    
    public long ProductId { get; set; }
    
    public long? CartId { get; set; }
    
    public static CartItem FromDto(CartItemCreateDTO cartItemCreateDTO)
    {
        return new CartItem
        {
            Accepted = cartItemCreateDTO.Accepted,
            Name = cartItemCreateDTO.Name,
            Quantity = cartItemCreateDTO.Quantity,
            Volume = cartItemCreateDTO.Volume,
            Price = cartItemCreateDTO.Price,
            ProductId = cartItemCreateDTO.ProductId,
            Removed = false
        };
    }
}