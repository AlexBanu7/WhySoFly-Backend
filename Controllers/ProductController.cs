using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDisplayDTO>>> GetProducts()
    {
        var products = await _context.Products.Include(p => p.NutritionalValues).ToListAsync();
        return products.Select(ProductDisplayDTO.ToDTO).ToList();
    }

    // GET: api/Product/5
    // <snippet_GetByID>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDisplayDTO>> GetProduct(long id)
    {
        var product = await _context.Products
            .Include(p => p.NutritionalValues)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }

        return ProductDisplayDTO.ToDTO(product);
    }

    // PUT: api/Product/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(long id, ProductUpdateDTO productUpdateDto)
    {
        if (id != productUpdateDto.Id)
        {
            return BadRequest();
        }
        
        var product = await _context.Products.FindAsync(id);

        if (product is not null)
        {
            _context.Entry(product).State = EntityState.Modified;
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
            return Ok(productUpdateDto);
        }
        else
        {
            return NotFound();
        }
    }

    // POST: api/Product
    [HttpPost]
    public async Task<ActionResult<Market>> CreateMarket(ProductCreateDTO productCreateDto)
    {
        var category = await _context.Categories.FindAsync(productCreateDto.CategoryId);
        
        if (category is null)
        {
            return NotFound();
        }
        
        var market = await _context.Markets.FindAsync(productCreateDto.MarketId);

        var nutritionalValuesCreateDTO = productCreateDto.NutritionalValues;

        var nutritionalValues = new NutritionalValues
        {
            Energy = nutritionalValuesCreateDTO.Energy,
            TotalFats = nutritionalValuesCreateDTO.TotalFats,
            SaturatedFats = nutritionalValuesCreateDTO.SaturatedFats,
            TransFats = nutritionalValuesCreateDTO.TransFats,
            TotalCarbohydrates = nutritionalValuesCreateDTO.TotalCarbohydrates,
            Fibers = nutritionalValuesCreateDTO.Fibers,
            Sugars = nutritionalValuesCreateDTO.Sugars,
            Proteins = nutritionalValuesCreateDTO.Proteins
        };
        
        var product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            PricePerQuantity = productCreateDto.PricePerQuantity,
            VolumePerQuantity = productCreateDto.VolumePerQuantity,
            SoldByWeight = productCreateDto.SoldByWeight,
            Image = productCreateDto.Image,
            Category = category,
            Market = market,
            NutritionalValues = nutritionalValues
        };
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}