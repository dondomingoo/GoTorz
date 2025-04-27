using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelPackagesController : ControllerBase
    {
        private readonly ITravelPackageService _service;

        public TravelPackagesController(ITravelPackageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _service.GetAllPackagesAsync();
            return Ok(packages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var package = await _service.GetPackageByIdAsync(id);
            if (package == null) return NotFound();
            return Ok(package);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TravelPackage package)
        {
            await _service.CreatePackageAsync(package);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeletePackageAsync(id);
            return NoContent();
        }
        // Search function
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> Search(
            string? destination = null,
            int? numberOfTravellers = null,
            DateTime? arrivalDate = null,
            DateTime? departureDate = null)
        {
            var packages = await _service.GetTravelPackagesAsync(destination, numberOfTravellers, arrivalDate, departureDate);
            return Ok(packages.ToList());
        }
    }


}
