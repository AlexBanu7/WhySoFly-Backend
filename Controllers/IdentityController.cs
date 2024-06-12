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
using Backend.Models;

namespace Backend.Controllers;

[Route("identity/")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IdentityController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
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
    public async Task<ActionResult> GetUser([FromBody] string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("User Name is required.");
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound($"User with user name {username} not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles[0];

        switch (role)
        {
            case "ADMIN":
                return Ok(new { user = UserDisplayDTO.ToDTO(user), role = roles[0] });
            case "Customer":
                return Ok(new { user = UserDisplayDTO.ToDTO(user), role = roles[0] });
            case "Manager":
                var market = await _context.Markets.FirstOrDefaultAsync(m => m.UserAccount == user);
                if (market == null)
                {
                    return NotFound($"Matching Market for user not found");
                }
                return Ok(new { user = UserDisplayDTO.ToDTO(user), role = roles[0], market = MarketDisplayDTO.ToDTO(market) });
            case "Employee":
                var employee = await _context.Employees
                    .Include(e => e.UserAccount)
                    .Include(e => e.Market)
                    .FirstOrDefaultAsync(e => e.UserAccount == user);
                if (employee == null)
                {
                    return NotFound($"Matching Employee account for user not found");
                }
                return Ok(new { user = UserDisplayDTO.ToDTO(user), role = roles[0], employee = EmployeeDisplayDTO.ToDTO(employee) });
        }

        return BadRequest("Invalid Role");
    }
    
    [HttpPatch("userName")]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameDTO changeUserNameDto)
    {
        var user = await _userManager.FindByEmailAsync(changeUserNameDto.Email);
        if (user == null)
        {
            return NotFound($"User with email {changeUserNameDto.Email} not found");
        }

        user.UserName = changeUserNameDto.NewUserName;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded ? Ok(new { success = true }) : StatusCode(500, $"{result.Errors.First().Description}");
    }
}