using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

// State string enum
public class State
{
    private State(string value) { Value = value; }

    public string Value { get; set; }

    public static State New { get { return new State("New"); } }
    public static State GatheringItems { get { return new State("Gathering Items"); } }
    public static State PreparingForApproval { get { return new State("Preparing For Approval"); } }
    public static State PendingApproval { get { return new State("Pending Approval"); } }
    public static State Approved { get { return new State("Approved"); } }
    public static State Finished { get { return new State("Finished"); } }
}

public class Cart
{
    public long Id { get; set; }
    
    public int Capacity { get; set; }
    
    public DateTime SubmissionDate { get; set; }
    
    public string State { get; set; }
    
    // Foreign Keys
    
    public long? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    
    public string? CustomerId { get; set; }
    public IdentityUser? Customer { get; set; }
    
    public long? MarketId { get; set; }
    public Market? Market { get; set; }
    
    public ICollection<CartItem> CartItems { get; set; }
}