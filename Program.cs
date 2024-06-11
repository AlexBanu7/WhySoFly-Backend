using System.Drawing.Printing;
using Backend;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<Backend.Services.WebSocketService>();

// Add DB contexts
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(@"Host=localhost:5432;Username=debug;Password=debug;Database=wsf"));

// Allow CORS Headers
builder.Services.AddCors(option =>
{
    option.AddPolicy("allowedOrigin",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

// Add Identity services to the container
builder.Services.AddAuthorization();

// Activate Identity APIs
builder.Services.AddIdentityApiEndpoints<IdentityUser>(
    options =>
        {
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        }
    )
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Map Identity routes
app.MapIdentityApi<IdentityUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Add roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Manager", "Employee", "Customer", "ADMIN" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        } 
    }
    
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var admin = await userManager.FindByEmailAsync("whysofly@gmail.com");
    if (admin == null)
    {
        var user = new IdentityUser
        {
            Email = "whysofly@gmail.com",
            UserName = "WhySoFly"
        };
        var result = await userManager.CreateAsync(user, "admin");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "ADMIN");
            await dbContext.SaveChangesAsync(); // Save changes after assigning role
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error: {error.Description}");
            }
        }
    }

    // Add Categories and migrate the database
    var categories = new[]
    {
        "Vegetables", 
        "Fruits", 
        "Meat and Seafood",
        "Dairy",
        "Bakery",
        "Frozen Foods",
        "Pantry Staples",
        "Snacks and Confectionery",
        "Beverages",
        "Cleaning Supplies",
        "Paper Products",
        "Kitchen Supplies",
        "Personal Care",
        "Health and Wellness",
        "Beauty",
        "Baby Food and Care",
        "Pet Supplies",
        "Home and Garden",
        "Electronics",
        "Toys and Games",
        "Car Care",
    };
    foreach (var categoryName in categories)
    {
        if (!await dbContext.Categories.AnyAsync(c => c.Name == categoryName))
        {
            await dbContext.Categories.AddAsync(new Category { Name = categoryName });
        }
    }

    await dbContext.SaveChangesAsync();
    dbContext.Database.Migrate();
}

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager,
        [FromBody] object empty) =>
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        }
        return Results.Unauthorized();
    })
    .WithOpenApi()
    .RequireAuthorization();

app.UseCors("allowedOrigin");

app.UseWebSockets();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();