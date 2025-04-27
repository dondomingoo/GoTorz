using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.Models;

public class SearchTravelPackageService : ISearchTravelPackageService
{
    private readonly HttpClient _httpClient;

    public SearchTravelPackageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TravelPackage>> GetTravelPackagesAsync(string? destination, int? numberOfTravellers, DateTime? arrivalDate, DateTime? departureDate)
    {
        string query = $"?destination={destination}&numberOfTravellers={numberOfTravellers}&arrivalDate={arrivalDate?.ToString("yyyy-MM-dd")}&departureDate={departureDate?.ToString("yyyy-MM-dd")}";
        return await _httpClient.GetFromJsonAsync<List<TravelPackage>>($"api/travelpackages/search{query}");
    }
}
