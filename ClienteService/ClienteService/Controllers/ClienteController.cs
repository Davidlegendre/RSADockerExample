using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClienteService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public IActionResult GetClienteData()
        {
            return Ok("Data for Cliente only");
        }
    }
}
