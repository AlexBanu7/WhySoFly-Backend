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
    public DbSet<Category> Categories { get; set; }
    public DbSet<NutritionalValues> NutritionalValues { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId);
        
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.UserAccount)
            .WithMany()
            .HasForeignKey(e => e.UserAccountId);
        
        modelBuilder.Entity<Market>()
            .HasOne(m => m.UserAccount)
            .WithMany()
            .HasForeignKey(m => m.UserAccountId);
        
        modelBuilder.Entity<Market>()
            .HasOne(m => m.StoreHours)
            .WithMany()
            .HasForeignKey(m => m.StoreHoursId);
    }
}