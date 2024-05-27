using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public EmployeeController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;

        }

        // Post: api/GetEmployeesByMarket
        [HttpPost("GetEmployeesByMarket")]
        public async Task<ActionResult<IEnumerable<EmployeeDisplayDTO>>> GetEmployees(GetEmployeesDTO getEmployeesDto)
        {
            var employeesQuery = _context.Employees
                .Include(e => e.UserAccount)
                .Include(e => e.Market)
                .Where(e => e.MarketId == getEmployeesDto.MarketId)
                .Where(e => (e.Status == Status.PendingApproval.Value) == getEmployeesDto.Pending);
            var employees = await employeesQuery.ToListAsync();
            return employees.Select(EmployeeDisplayDTO.ToDTO).ToList();
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee for given ID not found");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == employee.UserAccountId);
            if (user == null)
            {
                return NotFound("User account not found for given Employee");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
        
            // Remove all current roles from the user
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return StatusCode(500, $"Failed to remove roles from user");
            }
            await _userManager.AddToRoleAsync(user, "Customer");
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // POST : api/Market/claimInvite
        [HttpPost("claimInvite")]
        public async Task<IActionResult> ClaimInvite(ClaimInviteDTO claimInviteDto)
        {
            var market = await _context.Markets.FirstOrDefaultAsync(m => m.InvitationKey == claimInviteDto.InvitationKey);
            if (market == null)
            {
                return NotFound();
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == claimInviteDto.Email);
            if (user == null)
            {
                return NotFound();
            }
            
            var employee = new Employee
            {
                Market = market,
                UserAccount = user,
                Status = Status.PendingApproval.Value,
                OrdersDone = 0
            };
            
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
