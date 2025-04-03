using GoTorz.Api.Data;
using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelPackagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TravelPackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] TravelPackage package)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.TravelPackages.Add(package);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

}
