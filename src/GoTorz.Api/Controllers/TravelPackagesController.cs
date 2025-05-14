using GoTorz.Api.Data;
using GoTorz.Api.Services;
using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelPackagesController : ControllerBase
    {
        private readonly ITravelPackageService _service;
        private readonly ILogger<TravelPackagesController> _logger; 
        public TravelPackagesController(ITravelPackageService service, ILogger<TravelPackagesController> logger)
        {
            _service = service;
            _logger = logger;
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

        [Authorize(Roles = "Admin,SalesRep")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TravelPackage package)
        {
            try
            {
                await _service.CreatePackageAsync(package);
                _logger.LogInformation("TravelPackage for '{Destination}' created by user '{User}'", package.Destination, User.Identity?.Name);
                return Ok("Travel package created successfully.");
            }
            catch (Exception ex) 
            {
                _logger.LogWarning("Failed to create TravelPackage for '{Destination}', by use '{User}'", package.Destination, User.Identity
                        ?.Name);
                return BadRequest("Could not create travel package");
            }

        }

        [Authorize(Roles = "Admin,SalesRep")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeletePackageAsync(id);
                _logger.LogInformation("TravelPackage with TravelPackageId {Id} has been deleted", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("TravelPackage with TravelPackageId {Id} could not be deleted. Reason: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // Search function
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> Search(
            string? destination = null,
            DateTime? arrivalDate = null,
            DateTime? departureDate = null)
        {
            var packages = await _service.GetTravelPackagesAsync(destination, arrivalDate, departureDate);
            return Ok(packages.ToList());
        }
    }


}
