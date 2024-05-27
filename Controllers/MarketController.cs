using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MarketController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/Market
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarketDisplayDTO>>> GetMarkets()
        {
            var markets = await _context.Markets.Where(m => m.Verified && m.Employees != null).ToListAsync();
            return markets.Select(MarketDisplayDTO.ToDTO).ToList();
        }

        // GET: api/Market/5
        // <snippet_GetByID>
        [HttpGet("{id}")]
        public async Task<ActionResult<MarketDisplayDTO>> GetMarket(long id)
        {
            var market = await _context.Markets.FindAsync(id);

            if (market == null)
            {
                return NotFound();
            }

            return MarketDisplayDTO.ToDTO(market);
        }

        // PUT: api/Market/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMarket(long id, Market market)
        {
            if (id != market.Id)
            {
                return BadRequest();
            }

            _context.Entry(market).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Markets.Any(m => m.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(MarketDisplayDTO.ToDTO(market));
        }

        // POST: api/Market
        [HttpPost]
        public async Task<ActionResult<MarketDisplayDTO>> CreateMarket(MarketCreateDTO marketCreateDto)
        {
            (Market market, StoreHours storeHours) = marketCreateDto.ToMarketAndStoreHours();

            market.StoreHours = storeHours;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == marketCreateDto.Email);
            market.UserAccount = user;

            _context.Markets.Add(market);
            _context.StoreHours.Add(storeHours);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMarket),
                new { id = market.Id },
                MarketDisplayDTO.ToDTO(market));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarket(long id)
        {
            var market = await _context.Markets.FindAsync(id);
            if (market == null)
            {
                return NotFound();
            }

            _context.Markets.Remove(market);
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

        // POST : api/Market/approveRequest
        [HttpPost("approveRequest")]
        public async Task<IActionResult> ApproveRequest(ApproveRequestDTO approveRequestDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == approveRequestDto.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee of given ID not found!");
            }  
            
            employee.Status = Status.Available.Value;
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        // POST : api/Market/rejectRequest
        [HttpPost("rejectRequest")]
        public async Task<IActionResult> RejectRequest(ApproveRequestDTO rejectRequestDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == rejectRequestDto.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee of given ID not found!");
            }  
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == employee.UserAccountId);
            if (user == null)
            {
                return NotFound("Employee's User Account not found!");
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
            return Ok();
        }
    }
}
