using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OauthAspNetCore.Controllers
{
    [Route("api/home")]
    public class HomeController : Controller
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            var responseData = new
            {
                message = "This endpoint is public.",
                status = "success"
            };

            return Ok(responseData);
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            var responseData = new
            {
                message = "This endpoint is protected.",
                status = "success"
            };

            return Ok(responseData);
        }
    }
}
