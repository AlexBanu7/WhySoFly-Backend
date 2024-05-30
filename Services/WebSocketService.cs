using Backend.DTOs;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services;

public class WebSocketService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    
    public WebSocketService(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    public async Task<CommandResult> ExecuteCommand(MessageDTO receivedMessage, string socketEmail)
    {
        // Prepare message and destination
        var destination = socketEmail;
        var message = "";
        // Prepare objects
        var employee = new Employee();
        var cart = new Cart();
        var customer = new IdentityUser();
        // Find sender
        var sender = _context.Users.FirstOrDefault(c => c.Email == socketEmail);
        if (sender == null)
        {
            message = "User not found";
            return new CommandResult { Message = message, Destination = destination };
        }
        // Fetch cart, customer and employee
        var roles = await _userManager.GetRolesAsync(sender);
        var role = roles[0];
        if (role == "Customer")
        {
            customer = sender;
            cart = _context.Carts.FirstOrDefault(c => c.CustomerId == sender.Id && c.State == State.New.Value);
            if (cart == null)
            {
                message = "Cart not found";
                return new CommandResult { Message = message, Destination = destination };
            }
            if (cart.EmployeeId != null)
            {
                employee = _context.Employees.FirstOrDefault(e => e.Id == cart.EmployeeId);
                if (employee == null)
                {
                    message = "Employee for cart not found";
                    return new CommandResult { Message = message, Destination = destination };
                }
            }
        }
        else
        {
            employee = _context.Employees.FirstOrDefault(e => e.UserAccountId == sender.Id);
            if (employee == null)
            {
                message = "Employee for sender not found";
                return new CommandResult { Message = message, Destination = destination };
            }
            cart = _context.Carts.FirstOrDefault(c => c.EmployeeId == employee.Id && c.State == State.New.Value);
            if (cart == null)
            {
                message = "Cart not found";
                return new CommandResult { Message = message, Destination = destination };
            }
            customer = _context.Users.FirstOrDefault(c => c.Id == cart.CustomerId);
            if (customer == null)
            {
                message = "Customer for cart not found";
                return new CommandResult { Message = message, Destination = destination };
            }
        }
        // Route command
        var command = receivedMessage.Command;
        switch (command)
        {
            case "Checkout Customer":
                // Customer Checks out (POST /api/Cart)...
                // Customer notifies Socket...
                // Assign an available employee to the cart
                employee = _context.Employees.FirstOrDefault(e => e.MarketId == cart.MarketId && e.Status == Status.Available.Value);
                while (employee == null)
                {
                    Console.WriteLine("Retrying to find available employee");
                    Thread.Sleep(5000);
                    employee = _context.Employees.FirstOrDefault(e => e.Status == Status.Available.Value);
                }
                employee.Status = Status.Busy.Value;
                cart.EmployeeId = employee.Id;
                cart.State = State.GatheringItems.Value;
                await _context.SaveChangesAsync();
                // Send message to Employee
                message = "An order has been assigned to you.";
                destination = employee.UserAccount.Email;
                break;
            case "Add To Cart":
                // Customer adds a product to the cart (POST /api/CartItem)...
                // Customer notifies Socket...
                // Let the Employee know via socket notification
                message = "A new product has been added to the cart";
                destination = employee.UserAccount.Email;
                break;
            case "Fetched Products":
                // Employee fetches products physically and notifies socket
                // Message is relayed to the Customer
                message = "Products have been fetched";
                destination = customer.Email;
                break;
            case "Attached Photos":
                // Employee attaches verification photographs (TBI PATCH /api/CartItem - maybe in batch?)...
                // Move cart to Pending Approval state
                cart.State = State.PendingApproval.Value;
                await _context.SaveChangesAsync();
                // Notify the Customer via Socket
                message = "Verification Photographs have been attached";
                destination = customer.Email;
                break;
            case "Approve Product":
                // Customer Approves Product (PATCH /api/CartItem)...
                // Customer notifies Socket...
                // Let the Employee know via socket notification
                message = "A Product has been approved";
                destination = employee.UserAccount.Email;
                break;
            case "Reject Product":
                // Customer Rejects Product (PATCH /api/CartItem)...
                // Customer notifies Socket...
                // Let the Employee know via socket notification
                message = "A Product has been rejected";
                destination = employee.UserAccount.Email;
                break;
            case "Remove Product":
                // Customer removes a product from the cart (DELETE /api/CartItem)...
                // Customer notifies Socket...
                // Let the Employee know via socket notification
                message = "A Product has been removed by the Customer";
                destination = employee.UserAccount.Email;
                break;
            case "Confirm Cart":
                // Customer confirms cart via Socket
                cart.State = State.Finished.Value;
                employee.Status = Status.Break.Value;
                await _context.SaveChangesAsync();
                ExecuteDelayedActionAsync(async ()  =>
                {
                    employee.Status = Status.Available.Value;
                    await _context.SaveChangesAsync();
                });
                // Let the Employee know via socket notification
                message = "The Order has finished";
                destination = employee.UserAccount.Email;
                break;
            default:
                Console.WriteLine("Command not found.");
                break;
        }
        return new CommandResult { Message = message, Destination = destination };
    }
    
    public async Task ExecuteDelayedActionAsync(Action handleMessage)
    {
        // Delay for 5 minutes
        await Task.Delay(TimeSpan.FromMinutes(5));
        
        // Perform the action after the delay
        handleMessage();
    }
}