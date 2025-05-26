using Microsoft.AspNetCore.Mvc;

namespace TLD15.Endpoints;

public class PingController : Controller
{
    [HttpGet("ping")]
    public IActionResult Index()
    {
        return new JsonResult(new { Value = "pong" });
    }
}
