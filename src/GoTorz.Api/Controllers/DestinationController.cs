using GoTorz.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [Authorize(Roles = "Admin,SalesRep")]
    [ApiController]
    [Route("api/[controller]")]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _destinationService;

        public DestinationController(IDestinationService destinationService)
        {
            _destinationService = destinationService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchDestination([FromQuery] string query = "man")
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            try
            {
                var destinations = await _destinationService.SearchDestinationAsync(query);
                return Ok(destinations);
            }
            catch (Exception)
            {
                return StatusCode(502, "Error retrieving destination data.");
            }
        }
    }
}
