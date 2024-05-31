namespace Backend.DTOs;

public class AttachPhotosDTO
{
    
    public long CartId { get; set; }
    
    public ICollection<PhotoDTO> CartItems { get; set; }
}

public class PhotoDTO
{
    public long Id { get; set; }
    
    public string Image { get; set; }
}