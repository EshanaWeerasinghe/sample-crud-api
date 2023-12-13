// Import necessary namespaces
using Microsoft.EntityFrameworkCore;
using sample_crud_api.Models;

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Add CORS (Cross-Origin Resource Sharing) support
builder.Services.AddCors();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the database context with a connection string from appsettings.json
builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CRUDCS")));

// Build the web application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger and Swagger UI for API documentation in development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authorization (if needed)
app.UseAuthorization();

// Map controllers to the HTTP request pipeline
app.MapControllers();

// Start the application
app.Run();

// Perform database migration on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // Get the database context and apply migrations
        var dbContext = services.GetRequiredService<EmployeeContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log any errors that occur during database migration
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}