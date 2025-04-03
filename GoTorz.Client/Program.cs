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
            builder.Services.AddScoped<SearchTravelPackageService>();


            // Http
            builder.Services.AddScoped(sp => 
                new HttpClient { BaseAddress = new Uri("https://localhost:7111/") });

            // Authentication & Authorization
            builder.Services.AddScoped<LocalStorage>();
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>()); // Any time AuthenticationStateProvider needs to check AuthState it calls this method in our CustomAuthStateProvider
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
