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
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult> GetCategories(long? market_id)
        {
            if (market_id.HasValue)
            {
                var categories = await _context.Categories.Include(c => c.Products).ToListAsync();

                // Remove products that do not belong to the given market
                foreach (var category in categories)
                {
                    category.Products = category.Products.Where(p => p.MarketId == market_id.Value).ToList();
                }
                
                return Ok(categories.Select(CategoryDisplayDTO.ToDTO).ToList());
            }
            else
            {
                var categories = await _context.Categories.ToListAsync();
                return Ok(categories.Select(PlainCategoryDisplayDTO.ToDTO).ToList());
            }
        }

        // GET: api/Category/5
        // <snippet_GetByID>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDisplayDTO>> GetCategory(long id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return CategoryDisplayDTO.ToDTO(category);
        }
    }
}
