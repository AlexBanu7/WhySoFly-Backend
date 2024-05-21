using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MarketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Market
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Market>>> GetMarkets()
        {
            return await _context.Markets.Where(m => m.Verified).ToListAsync();
        }

        // GET: api/Market/5
        // <snippet_GetByID>
        [HttpGet("{id}")]
        public async Task<ActionResult<Market>> GetMarket(long id)
        {
            var market = await _context.Markets.FindAsync(id);

            if (market == null)
            {
                return NotFound();
            }

            return market;
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

            return Ok(market);
        }

        // POST: api/Market
        [HttpPost]
        public async Task<ActionResult<Market>> CreateMarket(MarketCreateDTO marketCreateDto)
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
                market);
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
        
        // POST : api/Market/claim_invite
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
