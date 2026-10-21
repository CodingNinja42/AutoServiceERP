using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServices.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestController : ControllerBase
{
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok(new { message = "JWT authentication successful!" });
    }
}