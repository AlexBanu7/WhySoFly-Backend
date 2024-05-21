using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
namespace Backend.Models;

// Status string enum
public class Status
{
    private Status(string value) { Value = value; }

    public string Value { get; set; }

    public static Status Available { get { return new Status("Available"); } }
    public static Status Busy { get { return new Status("Busy"); } }
    public static Status Break { get { return new Status("Break"); } }
    public static Status PendingApproval { get { return new Status("Pending Approval"); } }
}

public class Employee
{
    public long Id { get; set; }
    
    public string Status { get; set; }
    
    public int OrdersDone { get; set; }
    
    // Foreign Keys
    
    public Cart? Cart { get; set; }
    
    public long MarketId { get; set; }
    [JsonIgnore]
    public Market Market { get; set; }
    
    public string UserAccountId { get; set; }
    public IdentityUser UserAccount { get; set; }
}