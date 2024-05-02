using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }
    
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Market> Markets { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<StoreHours> StoreHours { get; set; }
    
    
}