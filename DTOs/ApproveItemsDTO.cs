namespace Backend.DTOs;

public class ApproveItemsDTO
{
    public long CartId { get; set; }
    
    public ICollection<ApproveItemDTO> CartItems { get; set; }
}

public class ApproveItemDTO
{
    public long Id { get; set; }
}