using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Api.Services.Auth;
using GoTorz.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using DotNetEnv;
using Microsoft.Extensions.Options;
using GoTorz.Api.Adapters;
using GoTorz.API.Data;

namespace GoTorz.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Retrieve BaseUrl from appsettings.json
            var apiBaseUrl = builder.Configuration.GetValue<string>("AppSettings:BaseUrl") ?? "https://localhost:7111";  // Default to localhost for dev

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

                    // Special handling for SignalR connections
                    // SignalR cannot use the Authorization header, so it passes the token as ?access_token=...
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            // Only apply this logic when connecting to the SignalR /supportchathub
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/supportchathub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
                      
            // Configuration bindings
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<RapidApiSettings>(builder.Configuration.GetSection("RapidApiSettings"));

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            
            // Authorization
            builder.Services.AddAuthorization();
          
            // Controllers & Swagger
            builder.Services.AddControllers();          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://gotorz-client-app-g4a8c3cgg9beckfy.swedencentral-01.azurewebsites.net",
                            "https://localhost:7272"
                        )
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Internal App Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, Services.Auth.TokenService>();
            builder.Services.AddScoped<ITravelPackageRepository, TravelPackageRepository>();
            builder.Services.AddScoped<ITravelPackageService, TravelPackageService>();
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IHotelService, HotelService>();
            builder.Services.AddScoped<IDestinationService, DestinationService>();
            builder.Services.AddScoped<IPaymentAdapter, StripePaymentAdapter>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();






            // External App Services (via HTTP)
            builder.Services.AddScoped<IBookingService, BookingService>(); // Stripe SDK internally uses HTTP - so external       
            builder.Services.AddHttpClient<IFlightApiAdapter, RapidApiFlightAdapter>();
            builder.Services.AddHttpClient<IHotelApiAdapter, RapidApiHotelAdapter>();
            builder.Services.AddHttpClient<IDestinationApiAdapter, RapidApiDestinationAdapter>();


            // System-level Services (HttpContextAccessor - for getting the current user)
            builder.Services.AddHttpContextAccessor();

            // SignalR
            builder.Services.AddSignalR();


            var app = builder.Build();

            // Ensure roles and users are seeded before the app runs
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await IdentitySeeder.SeedAsync(userManager, roleManager);  // Run the seeder
            }

            // Configure the HTTP request pipeline. 
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // --- CSP ---  // Maybe add later - CSP header can prevent most XSS attacks by restricting what scripts are allowed --- we would need to grant access to external APIs and Bootstrap etc.

            // --- Security ---
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();

            // --- Routing ---
            app.MapControllers();
            app.MapHub<SupportChatHub>("/supportchathub");

            app.Run();
        }
    }
}
