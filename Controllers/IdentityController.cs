using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Backend.Controllers;

[Route("identity/")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public IdentityController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost("assignCustomerRole")]
    public async Task<IActionResult> AssignCustomerRole([FromBody] string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email is required.");
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"User with email {email} not found");
        }

        var result = await _userManager.AddToRoleAsync(user, "Customer");
        return result.Succeeded ? Ok(new { success = true }) : StatusCode(500, $"Failed to assign Customer tole to user with email {email}");
    }
}