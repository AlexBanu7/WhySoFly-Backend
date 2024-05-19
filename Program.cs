using Backend;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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
    // Add roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Manager", "Employee", "Customer", "Manager_PENDING", "Employee_PENDING" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        } 
    }
    
    // Migrate db
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();