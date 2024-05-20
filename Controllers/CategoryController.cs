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
        public async Task<ActionResult<IEnumerable<CategoryDisplayDTO>>> GetCategories(long? market_id)
        {
            if (market_id.HasValue)
            {
                var categories = await _context.Categories.Include(c => c.Products).ToListAsync();

                // Remove products that do not belong to the given market
                foreach (var category in categories)
                {
                    category.Products = category.Products.Where(p => p.MarketId == market_id.Value).ToList();
                }
                
                return categories.Select(CategoryDisplayDTO.ToDTO).ToList();
            }
            else
            {
                // If no market_id is provided, return all categories with their products
                var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
                return categories.Select(CategoryDisplayDTO.ToDTO).ToList();
            }
        }

        // GET: api/Category/5
        // <snippet_GetByID>
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(long id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }
    }
}
