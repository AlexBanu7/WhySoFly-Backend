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
    
    [HttpPost("ByCustomerEmail")]
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
        
        cart.CartItems = cart.CartItems.Where(c => c.Removed == false).ToList();

        return CartDisplayDTO.ToDTO(cart);
    }
    
    [HttpPost("ByEmployeeId")]
    public async Task<ActionResult<CartDisplayDTO>> GetActiveCartForEmployee([FromBody] long employeeId)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        
        if (employee == null)
        {
            return NotFound("Employee not found for given Id");
        }
        
        var cart = await _context.Carts
            .Include(c => c.Customer)
            .Include(c => c.Employee)
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.EmployeeId == employee.Id && c.State != State.Finished.Value);
        
        if (cart == null)
        {
            return NotFound("No active cart found");
        }
        
        cart.CartItems = cart.CartItems.Where(c => c.Removed == false).ToList();
        
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
    
    [HttpPut("AttachPhotos")]
    public async Task<ActionResult> AttachPhotos(AttachPhotosDTO attachPhotosDto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == attachPhotosDto.CartId);
        
        if (cart == null)
        {
            return NotFound("Cart not found for given id");
        }
        
        foreach (var cartItem in cart.CartItems)
        {
            var image = attachPhotosDto.CartItems.FirstOrDefault(c => c.Id == cartItem.Id)?.Image;
            cartItem.Image = image != null ? Convert.FromBase64String(image) : null;
        }
        
        cart.State = State.PendingApproval.Value;
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet("FinishOrder/{cartId}")]
    public async Task<ActionResult> FinishOrder(long cartId)
    {
        var cart = await _context.Carts
            .FirstOrDefaultAsync(c => c.Id == cartId);
        
        if (cart == null)
        {
            return NotFound("Cart not found for given id");
        }
        
        cart.State = State.Finished.Value;
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost("ApproveItems")]
    public async Task<ActionResult> ApproveItems(ApproveItemsDTO approveItemsDto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == approveItemsDto.CartId);
        
        if (cart == null)
        {
            return NotFound("Cart not found for given id");
        }
        
        foreach (var cartItemDto in approveItemsDto.CartItems)
        {
            var cartItem = cart.CartItems.FirstOrDefault(c => c.Id == cartItemDto.Id);
            if (cartItem == null)
            {
                return NotFound("Cart item not found for given id");
            }
            cartItem.Accepted = true;
        }
        
        // check if all items are accepted
        if (cart.CartItems.All(c => c.Accepted))
        {
            cart.State = State.Removal.Value;
        }
        else
        {
            cart.State = State.PreparingForApproval.Value;
        }
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost("RemoveItems")]
    public async Task<ActionResult> RemoveItems(ApproveItemsDTO approveItemsDto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == approveItemsDto.CartId);
        
        if (cart == null)
        {
            return NotFound("Cart not found for given id");
        }
        
        foreach (var cartItemDto in approveItemsDto.CartItems)
        {
            var cartItem = cart.CartItems.FirstOrDefault(c => c.Id == cartItemDto.Id);
            if (cartItem == null)
            {
                return NotFound("Cart item not found for given id");
            }
            cartItem.Removed = true;
        }
        
        cart.State = State.Approved.Value;
        
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost("CartWithRemovedOnly")]
    public async Task<ActionResult<CartDisplayDTO>> GetCartWithRemovedItemsOnly([FromBody] long employeeId)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        
        if (employee == null)
        {
            return NotFound("Employee not found for given Id");
        }
        
        var cart = await _context.Carts
            .Include(c => c.Customer)
            .Include(c => c.Employee)
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.EmployeeId == employee.Id && c.State != State.Finished.Value);
        
        if (cart == null)
        {
            return NotFound("No active cart found");
        }
        
        cart.CartItems = cart.CartItems.Where(c => c.Removed == true).ToList();
        
        return CartDisplayDTO.ToDTO(cart);
    }
}