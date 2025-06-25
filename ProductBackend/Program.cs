using ProductBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Product Backend API", 
        Version = "v1",
        Description = "A minimal API for managing products"
    });
});

// Register the ProductService as a singleton since we're using in-memory storage
builder.Services.AddSingleton<ProductService>();

// Add CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Backend API v1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

// Add a root endpoint that provides API information
app.MapGet("/", () => new { 
    Message = "Product Backend API", 
    Version = "1.0.0",
    Documentation = "/swagger",
    Endpoints = new {
        Products = "/api/products",
        Health = "/health"
    }
});

Console.WriteLine("Starting Product Backend API...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("API Documentation available at: /swagger");

app.Run();