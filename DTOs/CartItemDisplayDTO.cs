using Backend.Models;

namespace Backend.DTOs;

public class CartItemDisplayDTO
{
    public long Id { get; set; }
    
    public bool Accepted { get; set; }
    
    public string Name { get; set; }
    
    public double Quantity { get; set; }
    
    public double Volume { get; set; }
    
    public double Price { get; set; }
    
    public byte[]? Image { get; set; }
    
    public long CartId { get; set; }
    
    public long ProductId { get; set; }
    
    public static CartItemDisplayDTO ToDTO(CartItem cartItem)
    {
        return new CartItemDisplayDTO
        {
            Id = cartItem.Id,
            Accepted = cartItem.Accepted,
            Name = cartItem.Name,
            Quantity = cartItem.Quantity,
            Volume = cartItem.Volume,
            Price = cartItem.Price,
            Image = cartItem.Image,
            CartId = cartItem.CartId,
            ProductId = cartItem.ProductId
        };
    }
}