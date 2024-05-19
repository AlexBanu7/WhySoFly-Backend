using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Backend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;

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
            return await _context.Markets.ToListAsync();
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
    }
}
