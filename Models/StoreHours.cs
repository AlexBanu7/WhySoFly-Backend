namespace Backend.Models;

public class StoreHours
{
    public long Id { get; set; }
    public string WorkDay { get; set; }
    public string Weekend { get; set; }
    
    // Foreign Keys
    
    public long MarketId { get; set; }
    public Market Market { get; set; }
}