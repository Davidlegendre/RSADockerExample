using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetClienteData()
        {
            return Ok("Data for Admin only");
        }
    }
}
