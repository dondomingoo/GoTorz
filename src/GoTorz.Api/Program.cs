using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace GoTorz.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //travelPackage builders
            builder.Services.AddScoped<ITravelPackageRepository, TravelPackageRepository>();
            builder.Services.AddScoped<ITravelPackageService, TravelPackageService>();

            builder.Services.AddScoped<IBookingService, BookingService>();

            // DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

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
            builder.Services.AddAuthentication(options => // "When someone makes a request and the controller says [Authorize], try to find a JWT in the request and validate it."
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => // "When using JWT Bearer tokens, these are the rules you must follow when validating the token."
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
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

            builder.Services.AddScoped<ITokenService, Services.Auth.TokenService>();

            // AuthService
            builder.Services.AddScoped<IAuthService, AuthService>();


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

            app.Run();
        }
    }
}
