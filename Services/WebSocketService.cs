using Backend.DTOs;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        List<string> destinations = new List<string>();
        List<string> messages = new List<string>();
        // Prepare objects
        var employee = new Employee();
        var cart = new Cart();
        var customer = new IdentityUser();
        // Find sender
        var sender = _context.Users.FirstOrDefault(c => c.Email == socketEmail);
        if (sender == null)
        {
            messages.Add("User not found");
            destinations.Add(socketEmail);
            return new CommandResult { Messages = messages, Destinations = destinations };
        }
        // Fetch cart, customer and employee
        var roles = await _userManager.GetRolesAsync(sender);
        var role = roles[0];
        if (role == "Customer")
        {
            customer = sender;
            cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CustomerId == sender.Id && c.State != State.Finished.Value);
            if (cart == null)
            {
                messages.Add("Cart not found");
                destinations.Add(socketEmail);
                return new CommandResult { Messages = messages, Destinations = destinations };
            }
            if (cart.EmployeeId != null)
            {
                employee = _context.Employees
                    .Include(e => e.UserAccount)
                    .FirstOrDefault(e => e.Id == cart.EmployeeId);
                if (employee == null)
                {
                    messages.Add("Employee for cart not found");
                    destinations.Add(socketEmail);
                    return new CommandResult { Messages = messages, Destinations = destinations };
                }
            }
        }
        else
        {
            employee = _context.Employees
                .Include(e => e.UserAccount)
                .FirstOrDefault(e => e.UserAccountId == sender.Id);
            if (employee == null)
            {
                messages.Add("Employee for sender not found");
                destinations.Add(socketEmail);
                return new CommandResult { Messages = messages, Destinations = destinations };
            }
            cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.EmployeeId == employee.Id && c.State != State.Finished.Value && c.State != State.Approved.Value);
            customer = _context.Users.FirstOrDefault(c => c.Id == cart.CustomerId);
            if (customer == null)
            {
                messages.Add("Customer for cart not found");
                destinations.Add(socketEmail);
                return new CommandResult { Messages = messages, Destinations = destinations };
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
                employee = _context.Employees
                    .Include(e => e.UserAccount)
                    .FirstOrDefault(e => e.MarketId == cart.MarketId && e.Status == Status.Available.Value);
                while (employee == null)
                {
                    Console.WriteLine("Retrying to find available employee");
                    Thread.Sleep(5000);
                    employee = _context.Employees
                        .Include(e => e.UserAccount)
                        .FirstOrDefault(e => e.Status == Status.Available.Value);
                }
                employee.Status = Status.Busy.Value;
                cart.EmployeeId = employee.Id;
                cart.State = State.GatheringItems.Value;
                await _context.SaveChangesAsync();
                // Send message to Customer
                messages.Add("An employee has been assigned to your cart");
                destinations.Add(customer.Email);
                // Send message to Employee
                messages.Add("An order has been assigned to you.");
                destinations.Add(employee.UserAccount.Email);
                break;
            case "Add To Cart":
                // Customer adds a product to the cart (POST /api/CartItem)...
                // Customer notifies Socket...
                // Let the Employee know via socket notification
                messages.Add("A new product has been added to the cart");
                destinations.Add(employee.UserAccount.Email);
                break;
            case "Fetched Products":
                // Employee fetches products physically and notifies socket
                // Message is relayed to the Customer
                // Move cart to Preparing For Approval state
                var isAlreadyAccepted = true;
                foreach (var item in cart.CartItems)
                {
                    if (item.Accepted != true)
                    {
                        isAlreadyAccepted = false;
                        break;
                    }
                }
                if (isAlreadyAccepted)
                {
                    cart.State = State.Removal.Value;
                }
                else
                {
                    cart.State = State.PreparingForApproval.Value;
                }
                await _context.SaveChangesAsync();
                messages.Add("Products have been fetched");
                destinations.Add(customer.Email);
                break;
            case "Attached Photos":
                // Employee attaches verification photographs (TBI PATCH /api/CartItem - maybe in batch?)...
                // Notify the Customer via Socket
                messages.Add("Verification Photographs have been attached");
                destinations.Add(customer.Email);
                break;
            case "Submitted Review":
                // Customer Rejects Product (PATCH /api/CartItem)...
                // Customer notifies Socket...
                // Let the Employee know via socket notification
                messages.Add("Products have been reviewed");
                destinations.Add(employee.UserAccount.Email);
                break;
            case "Confirm Cart":
                // Customer confirms cart via Socket
                // Let the Employee know via socket notification
                messages.Add("The Cart has been confirmed");
                destinations.Add(employee.UserAccount.Email);
                break;
            case "Finish Order":
                // Customer confirms cart via Socket
                // Let the Employee know via socket notification
                messages.Add("The Order has finished!");
                destinations.Add(customer.Email);
                break;
            default:
                Console.WriteLine("Command not found.");
                messages.Add("Command not found");
                destinations.Add(socketEmail);
                break;
        }
        return new CommandResult { Messages = messages, Destinations = destinations };
    }
}