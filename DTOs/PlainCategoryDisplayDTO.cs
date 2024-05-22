using Backend.Models;

namespace Backend.DTOs;

public class PlainCategoryDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    
    public static PlainCategoryDisplayDTO ToDTO(Category category)
    {
        return new PlainCategoryDisplayDTO
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
}