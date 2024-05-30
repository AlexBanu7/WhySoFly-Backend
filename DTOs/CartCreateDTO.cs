using Backend.Models;

namespace Backend.DTOs;

public class CartCreateDTO
{
    // Foreign Keys
    public string CustomerEmail { get; set; }
    
    public long MarketId { get; set; }
    
    public ICollection<CartItemCreateDTO> CartItems { get; set; }
    
    public static Cart FromDto(CartCreateDTO cartCreateDTO)
    {
        return new Cart
        {
            Capacity = 300,
            SubmissionDate = DateTime.Now.ToUniversalTime(),
            State = State.New.Value,
            MarketId = cartCreateDTO.MarketId,
            CartItems = cartCreateDTO.CartItems.Select(CartItemCreateDTO.FromDto).ToList()
        };
    }
}