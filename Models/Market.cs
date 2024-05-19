using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class Market
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Latitude { get; set; }
    
    public string Longitude { get; set; }
    
    // Foreign Keys

    public ICollection<Employee>? Employees { get; set; }

    public ICollection<Product>? Products { get; set; }
    
    public long? StoreHoursId { get; set; }
    public StoreHours? StoreHours { get; set; }
    
    public string? UserAccountId { get; set; }
    public IdentityUser? UserAccount { get; set; }
}