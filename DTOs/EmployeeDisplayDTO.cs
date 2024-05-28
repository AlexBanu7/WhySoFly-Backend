using Backend.Models;

namespace Backend.DTOs;

public class EmployeeDisplayDTO
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Status { get; set; }
    
    public int OrdersDone { get; set; }
    
    public string MarketName { get; set; }
    
    public UserDisplayDTO UserAccount { get; set; }
    
    public static EmployeeDisplayDTO ToDTO(Employee employee)
    {
        return new EmployeeDisplayDTO
        {
            Name = employee.UserAccount is null ? null : employee.UserAccount.UserName,
            Id = employee.Id,
            Status = employee.Status,
            OrdersDone = employee.OrdersDone,
            MarketName = employee.Market.Name,
            UserAccount = employee.UserAccount is null ? null : UserDisplayDTO.ToDTO(employee.UserAccount)
        };
    }
}