using GoTorz.Client.Settings;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GoTorz.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Http

            // Http
            var apiSettings = new ApiSettings();
            builder.Configuration.Bind(apiSettings);
            builder.Services.AddSingleton(apiSettings);

            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri(apiSettings.ApiBaseUrl) });                                           // HttpClient now automatically gets the base URL

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
