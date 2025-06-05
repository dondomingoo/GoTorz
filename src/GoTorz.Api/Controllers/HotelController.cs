using GoTorz.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [Authorize(Roles = "Admin,SalesRep")]
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet("search-hotels")]
        public async Task<IActionResult> SearchHotels(
            [FromQuery] string destId,
            [FromQuery] string checkin,
            [FromQuery] string checkout,
            [FromQuery] int adults = 1,
            [FromQuery] string children = "")
        {
            if (string.IsNullOrWhiteSpace(destId))
                return BadRequest("Destination ID is required.");

            try
            {
                var hotels = await _hotelService.SearchHotelsAsync(destId, checkin, checkout, adults, children);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(502, "Error retrieving hotel data.");
            }
        }
    }
}
