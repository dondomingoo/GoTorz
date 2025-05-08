using GoTorz.Api.Adapters;
using GoTorz.Shared.DTOs.Travelplanner;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GoTorz.Api.Services
{
    public class DestinationService : IDestinationService
    {
        private readonly IDestinationApiAdapter _destinationApiAdapter;

        public DestinationService(IDestinationApiAdapter destinationApiAdapter)
        {
            _destinationApiAdapter = destinationApiAdapter;
        }

        public async Task<List<DestinationDto>> SearchDestinationAsync(string query)
        {
            // Delegate the actual API interaction to the adapter
            return await _destinationApiAdapter.SearchDestinationAsync(query);
        }
    }
}
