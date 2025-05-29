using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp2.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FeeController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFee()
        {
            var username = HttpContext.User.Identity.Name;

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok("fee operations " + username + ", " + userId);
        }
    }
}
