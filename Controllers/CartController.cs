using System.Net.WebSockets;
using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("CartByCustomerEmail")]
    public async Task<ActionResult<CartDisplayDTO>> GetActiveCartForCustomer([FromBody] string customerEmail)
    {
        Console.WriteLine("Sending regards to the customer!");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == customerEmail);
        
        if (user == null)
        {
            return NotFound("User not found for given email");
        }
        
        var cart = await _context.Carts
            .Include(c => c.Customer)
            .Include(c => c.Employee)
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.State != State.Finished.Value);
        
        if (cart == null)
        {
            return NotFound("No active cart found");
        }
        
        return CartDisplayDTO.ToDTO(cart);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCart(CartCreateDTO cartCreateDto)
    {
        var cart = CartCreateDTO.FromDto(cartCreateDto);
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == cartCreateDto.CustomerEmail);
        
        if (user == null)
        {
            return NotFound("User not found for given email");
        }
        
        cart.CustomerId = user.Id;
        
        _context.Carts.Add(cart);
        _context.CartItems.AddRange(cart.CartItems);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost("CartItem")]
    public async Task<ActionResult> AddCartItem(CartItemCreateDTO cartItemCreateDto)
    {
        var cartItem = CartItemCreateDTO.FromDto(cartItemCreateDto);
        
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == cartItemCreateDto.CartId);
        
        if (cart == null)
        {
            return NotFound("Cart not found for given id");
        }
        
        cart.CartItems.Add(cartItem);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}