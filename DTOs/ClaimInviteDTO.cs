namespace Backend.DTOs;

public class ClaimInviteDTO
{
    public string Email { get; set; }
    
    public Guid InvitationKey { get; set; }
}