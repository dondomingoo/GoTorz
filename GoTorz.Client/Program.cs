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
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            
            //service
            builder.Services.AddScoped<ISearchTravelPackageService, SearchTravelPackageService>();


            // Http
            builder.Services.AddScoped(sp => 
                new HttpClient { BaseAddress = new Uri("https://localhost:7111/") });
            //travelpackage etc.
            builder.Services.AddScoped<IHotelService, HotelService>();
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IDestinationService, DestinationService>();
            builder.Services.AddScoped<ITravelService, TravelService>();
            builder.Services.AddScoped<IFlightDestinationService, FlightDestinationService>();
            builder.Services.AddScoped<TravelPriceCalculator>();



            // Authentication & Authorization
            builder.Services.AddScoped<LocalStorage>();
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>()); // Any time AuthenticationStateProvider needs to check AuthState it calls this method in our CustomAuthStateProvider
            builder.Services.AddScoped<IClientAuthService, ClientAuthService>();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
