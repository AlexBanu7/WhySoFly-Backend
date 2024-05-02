using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class Employee
{
    public long Id { get; set; }
    
    public string State { get; set; }
    
    public float DailyTips { get; set; }
    
    // Foreign Keys
    
    public Cart Cart { get; set; }
    
    public long MarketId { get; set; }
    public Market Market { get; set; }
    
    public Guid UserAccountId { get; set; }
    public IdentityUser UserAccount { get; set; }
}