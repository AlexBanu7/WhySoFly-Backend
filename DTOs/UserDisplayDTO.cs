using Microsoft.AspNetCore.Identity;

namespace Backend.DTOs;

public class UserDisplayDTO
{
    public Guid Id { get; set; }
    
    public string? UserName { get; set; }
    
    public string? NormalizedUserName { get; set; }
    
    public string? Email { get; set; }
    
    public string? NormalizedEmail { get; set; }
    
    public bool EmailConfirmed { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public bool PhoneNumberConfirmed { get; set; }
    
    public static UserDisplayDTO ToDTO(IdentityUser user)
    {
        return new UserDisplayDTO
        {
            Id = Guid.Parse(user.Id),
            UserName = user.UserName,
            NormalizedUserName = user.NormalizedUserName,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed
        };
    }
}