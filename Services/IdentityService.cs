using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Writers;

namespace Backend.Services;

public class IdentityService
{

    private UserManager<IdentityUser> userManager;

    public IdentityService(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }
    
    public async void AssignRoleByEmail(string email, string role)
    {
        
    }
}