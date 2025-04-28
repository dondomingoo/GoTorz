using Microsoft.AspNetCore.Mvc;
using IFlightServiceBackend = GoTorz.Api.Services.IFlightService;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightSearchController : ControllerBase
    {
        private readonly IFlightServiceBackend _flightService;

        public FlightSearchController(IFlightServiceBackend flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("search-flights")]
        public async Task<IActionResult> SearchFlights(
            [FromQuery] string fromId,
            [FromQuery] string toId,
            [FromQuery] string departureDate,
            [FromQuery] string returnDate,
            [FromQuery] int adults = 1,
            [FromQuery] string children = "")
        {
            try
            {
                var result = await _flightService.SearchFlightsAsync(
                    fromId, toId, departureDate, returnDate, adults, children);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(502, "Error retrieving flight search results.");
            }
        }
    }
}
