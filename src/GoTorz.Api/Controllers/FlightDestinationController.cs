using GoTorz.Api.Services;
using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using IFlightServiceBackend = GoTorz.Api.Services.IFlightService;


namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightServiceBackend _flightService;

        public FlightsController(IFlightServiceBackend flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("search-flight-destinations")]
        public async Task<IActionResult> SearchFlightDestinations([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            try
            {
                var results = await _flightService.SearchFlightDestinationsAsync(query);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(502, "Error retrieving flight destinations.");
            }
        }
    }
}
