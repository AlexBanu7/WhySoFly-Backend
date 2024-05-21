using Backend.Models;

namespace Backend.DTOs;

public class MarketCreateDTO
{
    public string Name { get; set; }
    
    public string Latitude { get; set; }
    
    public string Longitude { get; set; }
    
    public string WorkDay { get; set; }
    
    public string Weekend { get; set; }
    
    public string Email { get; set; }

    public (Market, StoreHours) ToMarketAndStoreHours()
    {
        return (new Market
        {
            Name = Name,
            Latitude = Latitude,
            Longitude = Longitude,
            InvitationKey = Guid.NewGuid(),
            Verified = false
        }, new StoreHours
        {
            WorkDay = WorkDay,
            Weekend = Weekend
        });
    }
}