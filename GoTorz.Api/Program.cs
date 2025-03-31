using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GoTorz.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
           
            builder.Services.Configure<IdentityOptions>(options => // CHANGE LATER - We can delete this if we want defaults
            {
                options.Password.RequireDigit = false;             // Default true
                options.Password.RequiredLength = 1;               // Default 6
                options.Password.RequireNonAlphanumeric = false;   // Default true
                options.Password.RequireUppercase = false;         // Default true
                options.Password.RequireLowercase = false;         // Default true
                options.Password.RequiredUniqueChars = 0;          // Default 1
            });
        
            // Authentication (Jwt)
            builder.Services.AddAuthentication(options => // When a controller or endpoint has [Authorize], ASP.NET will check if a valid JWT token is attached.
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => // Token validation rules for all JWT tokens received (both HTTP and WebSocket)
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],      // Must match token issuer
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],  // Must match token audience
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),

                        // Maps claims to Identity fields for easy access in controllers and SignalR hubs
                        NameClaimType = ClaimTypes.Name,            // Allows using User.Identity.Name
                        RoleClaimType = ClaimTypes.Role             // Allows using User.IsInRole("Admin")
                    };

                    // Special handling for SignalR connections
                    // SignalR cannot use the Authorization header, so it passes the token as ?access_token=...
                    options.Events = new JwtBearerEvents            
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            // Only apply this logic when connecting to the SignalR /chathub
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/chathub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
                      
            // Configuration binding (JWT)
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // Authorization
            builder.Services.AddAuthorization();
          
            // Controllers & Swagger
            builder.Services.AddControllers();          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ClientPolicy", policy =>
                {
                    policy.WithOrigins("https://localhost:7272")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            
            // TokenService
            builder.Services.AddScoped<ITokenService, TokenService>();

            // SignalR
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline. 
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // --- CSP ---  // Maybe add later - CSP header can prevent most XSS attacks by restricting what scripts are allowed --- we would need to grant access to external APIs and Bootstrap etc.

            // --- Security ---
            app.UseHttpsRedirection();
            app.UseCors("ClientPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            // --- Routing ---
            app.MapControllers();
            app.MapHub<SupportChatHub>("/chathub");  // This becomes your SignalR endpoint

            app.Run();
        }
    }
}
