using BookingService.Data;
using BookingService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IEventPublisher, EventPublisher>();


builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("CustomerDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("CustomerDb"))
    ));

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
