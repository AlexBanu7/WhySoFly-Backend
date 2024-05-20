using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTOs;
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
    
    [HttpPost("assignRole")]
    public async Task<IActionResult> AssignRole(RoleAssignDTO roleAssignDto)
    {

        var user = await _userManager.FindByEmailAsync(roleAssignDto.email);
        if (user == null)
        {
            return NotFound($"User with email {roleAssignDto.email} not found");
        }
        
        // Get the list of roles the user currently has
        var currentRoles = await _userManager.GetRolesAsync(user);
        
        // Remove all current roles from the user
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return StatusCode(500, $"Failed to remove roles from user with email {roleAssignDto.email}");
        }

        var result = await _userManager.AddToRoleAsync(user, roleAssignDto.role);
        return result.Succeeded ? Ok(new { success = true }) : StatusCode(500, $"Failed to assign {roleAssignDto.role} role to user with email {roleAssignDto.email}");
    }
    
    [HttpPost("userInfo")]
    [Authorize]
    public async Task<ActionResult> GetUser([FromBody] string email)
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

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new { user = user, role = roles[0] });
    }
}