using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GoTorz.Shared.Models;

public class SearchTravelPackageService
{
    private readonly HttpClient _httpClient;

    public SearchTravelPackageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TravelPackage>> GetTravelPackagesAsync(string? destination, DateTime? arrivalDate, DateTime? departureDate)
    {
        string query = $"?destination={destination}&arrivalDate={arrivalDate?.ToString("yyyy-MM-dd")}&departureDate={departureDate?.ToString("yyyy-MM-dd")}";
        return await _httpClient.GetFromJsonAsync<List<TravelPackage>>($"api/travelpackage{query}");
    }
}
