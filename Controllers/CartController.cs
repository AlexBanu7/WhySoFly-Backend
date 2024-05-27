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
        
        var employee = await _context.Employees
            .Include(e => e.UserAccount)
            .FirstOrDefaultAsync(e => e.MarketId == cart.MarketId && e.Status == Status.Available.Value);
        
        if (employee == null)
        {
            return NotFound("No available employee found for given market");
        }
        
        cart.EmployeeId = employee.Id;
        
        _context.Carts.Add(cart);
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }

}