using System.Security.Claims;
using System.Text;
using CustomerService.Data;
using CustomerService.Models;
using CustomerService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("CustomerDb"),
        new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddSingleton<AuthService>(provider =>
    new AuthService(builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine("JWT Secret is null or empty!");
}
else
{
    Console.WriteLine($"JWT Secret loaded from config: '{jwtSecret}' (Length: {jwtSecret.Length})");
}
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            // Map claims correctly
            NameClaimType = ClaimTypes.NameIdentifier,  // <-- Use standard claim type here
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception}");
                Console.WriteLine($"Token: {context.Request.Headers["Authorization"]}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer Service", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await db.Database.MigrateAsync();
}

// Demo: Generate and show JWT token for a test user
/*
var authService = app.Services.GetRequiredService<AuthService>();

var testUser = new Customer
{
    Id = 1,
    Email = "testuser@example.com",
    FirstName = "Test",
    Surname = "User",
    PasswordHash = authService.HashPassword(null!, "TestPassword123!")
};*/
/*
var token = authService.GenerateJwtToken(testUser);

Console.WriteLine("Generated JWT token for test user:");
Console.WriteLine(token);
*/
// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        var token = authHeader.ToString().Replace("Bearer ", "");
        Console.WriteLine($"Raw token received: {token}");
        Console.WriteLine($"Token contains {token.Count(c => c == '.')} dots.");
    }
    else
    {
        Console.WriteLine("No Authorization header received.");
    }
    await next.Invoke();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();