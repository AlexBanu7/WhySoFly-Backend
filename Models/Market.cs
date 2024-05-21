using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
namespace Backend.Models;

public class Market
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Latitude { get; set; }
    
    public string Longitude { get; set; }
    
    public Guid InvitationKey { get; set; }
    
    public bool Verified { get; set; }
    
    // Foreign Keys
    
    
    [JsonIgnore]
    public ICollection<Employee>? Employees { get; set; }

    [JsonIgnore]
    public ICollection<Product>? Products { get; set; }
    
    public long? StoreHoursId { get; set; }
    public StoreHours? StoreHours { get; set; }
    
    public string? UserAccountId { get; set; }
    public IdentityUser? UserAccount { get; set; }
}