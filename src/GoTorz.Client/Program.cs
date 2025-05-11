using GoTorz.Client.Services;
using GoTorz.Client.Services.Helpers;
using GoTorz.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GoTorz.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");


            //service
            builder.Services.AddScoped<ISearchTravelPackageService, SearchTravelPackageService>();
            builder.Services.AddScoped<IBookingHistoryservice, BookingHistoryService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();


            // Http
            var apiBaseUrl = "__API_BASE_URL__";

            // Use localhost for local dev if not replaced
            if (apiBaseUrl == "__API_BASE_URL__")
            {
                apiBaseUrl = "https://localhost:7111/";
            }

            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri(apiBaseUrl) });



            //travelpackage etc.
            builder.Services.AddScoped<IHotelService, HotelService>();
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IDestinationService, DestinationService>();
            builder.Services.AddScoped<ITravelService, TravelService>();
            builder.Services.AddScoped<IFlightDestinationService, FlightDestinationService>();
            builder.Services.AddScoped<TravelPriceCalculator>();

            // Authentication & Authorization
            builder.Services.AddScoped<ILocalStorage, LocalStorage>();
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<ICustomAuthStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>()); // Inject this in components (not the interface or concrete type)
            builder.Services.AddScoped<IClientAuthService, ClientAuthService>();           
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
