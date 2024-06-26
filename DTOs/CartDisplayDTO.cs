﻿using Backend.Models;

namespace Backend.DTOs;

public class CartDisplayDTO
{
    public long Id { get; set; }
    
    public int Capacity { get; set; }
    
    public DateTime SubmissionDate { get; set; }
    
    public string State { get; set; }
    
    // Foreign Keys
    
    public long? EmployeeId { get; set; }
    
    public string EmployeeName { get; set; }
    
    public string? CustomerId { get; set; }
    
    public long? MarketId { get; set; }
    
    public string?  MarketName { get; set; }
    
    public ICollection<CartItemDisplayDTO> CartItems { get; set; }
    
    public static CartDisplayDTO ToDTO(Cart cart)
    {
        return new CartDisplayDTO
        {
            Id = cart.Id,
            Capacity = cart.Capacity,
            SubmissionDate = cart.SubmissionDate,
            State = cart.State,
            EmployeeId = cart.EmployeeId,
            EmployeeName = cart.Employee != null ? cart.Employee.UserAccount.UserName : "Unknown",
            CustomerId = cart.CustomerId,
            CartItems = cart.CartItems.Select(CartItemDisplayDTO.ToDTO).ToList(),
            MarketId = cart.MarketId,
            MarketName = cart.Market != null ? cart.Market.Name : "Unknown"
        };
    }
}