using Microsoft.AspNetCore.Mvc;
using GoTorz.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            GetAll();
        }

        // Loads all travel packages initially when opening the page
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> GetAll()
        {
            var packages = await _travelPackageService.GetAllTravelPackagesAsync();
            return Ok(packages.ToList());
        }

        // Search function
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
