using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with MySQL
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36)))
);

// Register services
builder.Services.AddScoped<ICabFareService, CabFareService>();
builder.Services.AddScoped<IPricingCalculator, PricingCalculator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add logging (built-in by default, but this enables more detailed logs if needed)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Enable detailed error pages only in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // In production, use exception handler middleware to handle exceptions gracefully
    app.UseExceptionHandler("/error");
}

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentService v1");
});

// Map controllers
app.MapControllers();

// Optional: You can map a fallback error endpoint (for production)
app.Map("/error", () => Results.Problem("An unexpected error occurred. Please try again later."));

app.Run();
