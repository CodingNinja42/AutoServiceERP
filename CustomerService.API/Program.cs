
using CustomerServices.Application.Interfaces;
using CustomerServices.Infrastructure.Data;
using CustomerServices.Infrastructure.Repositories;
using CustomerServices.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

// Create the application builder and load configuration.
var builder = WebApplication.CreateBuilder(args);

// Register controller support so the application can handle API requests.
builder.Services.AddControllers();

// Register API Explorer to discover controller endpoints for Swagger.
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger and enable JWT Bearer authentication in its UI.
builder.Services.AddSwaggerGen(options =>
{
    // Define the Bearer authentication scheme used by protected endpoints.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste your JWT access token here."
    });

    // Tell Swagger to send the Bearer token with API requests.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register the Customer SQL Server database context using appsettings.json.
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("CustomerConnection")));

// Register the repository implementation for database operations.
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Register the service implementation for customer business logic.
builder.Services.AddScoped<ICustomerService, CustomerServiceImplementation>();

// Read JWT settings from configuration and fail clearly if the key is missing.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing in configuration.");

// Configure JWT Bearer authentication to validate incoming access tokens.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Accept tokens issued by the configured AuthService issuer.
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            // Accept tokens intended for this application's configured audience.
            ValidateAudience = true,
            ValidAudience = jwtAudience,

            // Verify the token signature using the configured signing key.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            // Reject expired tokens, allowing a small clock difference.
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// Register authorization services for endpoints protected by [Authorize].
builder.Services.AddAuthorization();

// Build the configured application.
var app = builder.Build();

// Enable Swagger only in the Development environment.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS when HTTPS redirection is configured.
app.UseHttpsRedirection();

// Authenticate requests before checking their authorization permissions.
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes such as /api/Customers.
app.MapControllers();

// Start the web application.
app.Run();
