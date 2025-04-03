using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using GoTorz.Api.Services;

namespace GoTorz.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelPackageController : ControllerBase
    {
        private readonly ITravelPackageService _travelPackageService;

        public TravelPackageController(ITravelPackageService travelPackageService)
        {
            _travelPackageService = travelPackageService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] TravelPackage travelPackage)
        {
            if (travelPackage == null || !ModelState.IsValid)
                return BadRequest("Invalid travel package data.");

            // You can generate the ID in service or client — here is one way
            travelPackage.TravelPackageId = "TP" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            var created = await _travelPackageService.CreateAsync(travelPackage);
            return CreatedAtAction(nameof(Get), new { id = created.TravelPackageId }, created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TravelPackage>> Get(string id)
        {
            var package = await _travelPackageService.GetByIdAsync(id);
            return package is not null ? Ok(package) : NotFound("Travel package not found.");
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> List()
        {
            var packages = await _travelPackageService.GetAllTravelPackagesAsync();
            return Ok(packages.ToList());
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] TravelPackage updated)
        {
            var result = await _travelPackageService.UpdateAsync(updated);
            return result ? NoContent() : NotFound("Travel package not found.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var result = await _travelPackageService.DeleteAsync(id);
            return result ? NoContent() : NotFound("Travel package not found.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> GetAll()
        {
            var packages = await _travelPackageService.GetAllTravelPackagesAsync();
            return Ok(packages.ToList());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> Search(
            string? destination = null,
            DateTime? arrivalDate = null,
            DateTime? departureDate = null)
        {
            var packages = await _travelPackageService.GetTravelPackagesAsync(destination, arrivalDate, departureDate);
            return Ok(packages.ToList());
        }
    }
}
