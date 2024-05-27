using Backend.Models;

namespace Backend.DTOs;

public class CartItemCreateDTO
{
    public string Name { get; set; }
    
    public double Quantity { get; set; }
    
    public double Volume { get; set; }
    
    public double Price { get; set; }
    
    // Foreign Keys
    
    public long ProductId { get; set; }
    
    public static CartItem FromDto(CartItemCreateDTO cartItemCreateDTO)
    {
        return new CartItem
        {
            Accepted = false,
            Name = cartItemCreateDTO.Name,
            Quantity = cartItemCreateDTO.Quantity,
            Volume = cartItemCreateDTO.Volume,
            Price = cartItemCreateDTO.Price,
            ProductId = cartItemCreateDTO.ProductId
        };
    }
}