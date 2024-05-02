using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class Market
{
    public long Id { get; set; }
    
    public string Location { get; set; }
    
    // Foreign Keys

    public ICollection<Employee>? Employees { get; set; }

    public ICollection<Product>? Products { get; set; }
    
    public StoreHours? StoreHours { get; set; }
    
    public Guid UserAccountId { get; set; }
    public IdentityUser? UserAccount { get; set; }
}