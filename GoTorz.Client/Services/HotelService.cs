using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs;
using System.Net.Http.Json;

namespace GoTorz.Client.Services
{

    public class HotelService : IHotelService
    {
        private readonly HttpClient _http;

        public HotelService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<HotelDto>> SearchHotelsAsync(string destId, DateTime checkin, DateTime checkout, int adults, List<int> children)
        {
            string childrenAges = string.Join(",", children);
            var url = $"api/hotels/search-hotels?destId={destId}&checkin={checkin:yyyy-MM-dd}&checkout={checkout:yyyy-MM-dd}&adults={adults}&children={childrenAges}";
            return await _http.GetFromJsonAsync<List<HotelDto>>(url) ?? new();
        }
    }
}
