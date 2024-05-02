using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class Cart
{
    public long Id { get; set; }
    
    public int Capacity { get; set; }
    
    public DateTime SubmissionDate { get; set; }
    
    public string State { get; set; }
    
    // Foreign Keys
    
    public long EmployeeId { get; set; }
    public Employee Employee { get; set; }
    
    public Guid CustomerId { get; set; }
    public IdentityUser Customer { get; set; }
    
    public ICollection<CartItem> CartItems { get; set; }
}