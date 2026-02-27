using Microsoft.AspNetCore.Mvc;

namespace tavern_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PingController
{
    [HttpGet]
    public IActionResult Ping()
    {
        return new JsonResult(new { message = "pong" });
    }
}
