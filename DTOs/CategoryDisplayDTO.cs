using Backend.Models;

namespace Backend.DTOs;

public class CategoryDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public ICollection<CategoryProductDisplayDTO> Products { get; set; }
    
    public static CategoryDisplayDTO ToDTO(Category category)
    {
        return new CategoryDisplayDTO
        {
            Id = category.Id,
            Name = category.Name,
            Products = category.Products.Select(CategoryProductDisplayDTO.ToDTO).ToList()
        };
    }
}