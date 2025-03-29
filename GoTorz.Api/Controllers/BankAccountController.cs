using GoTorz.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        [Authorize(Roles = "Admin")] // TEST OF AUTH CONTROLLAAAH!
        [HttpGet]
        public ActionResult<IEnumerable<BankAccount>> GetAccounts()
        {
            var accounts = new List<BankAccount>
            {
                new() { Id = 1, Owner = "Alice", Balance = 1200.50m },
                new() { Id = 2, Owner = "Bob", Balance = 850.75m }
            };

            return Ok(accounts);
        }
    }
}
