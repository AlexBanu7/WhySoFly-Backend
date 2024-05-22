using Backend.Models;

namespace Backend.DTOs;

public class MarketDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Latitude { get; set; }
    
    public string Longitude { get; set; }
    
    public Guid InvitationKey { get; set; }
    
    public bool Verified { get; set; }
    
    public StoreHours? StoreHours { get; set; }
    
    public UserDisplayDTO UserAccount { get; set; }
    
    public static MarketDisplayDTO ToDTO(Market market)
    {
        return new MarketDisplayDTO
        {
            Id = market.Id,
            Name = market.Name,
            Latitude = market.Latitude,
            Longitude = market.Longitude,
            InvitationKey = market.InvitationKey,
            Verified = market.Verified,
            StoreHours = market.StoreHours,
            UserAccount = UserDisplayDTO.ToDTO(market.UserAccount)
        };
    }
}