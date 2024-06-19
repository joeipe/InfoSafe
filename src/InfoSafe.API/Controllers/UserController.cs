using InfoSafe.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserService _userService;

        public AuthController(
            ILogger<AuthController> logger,
            UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet()]
        [Authorize(Policy = "IsAdmin")]
        public async Task<ActionResult> GetClaims()
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(AuthController));
            scopeInfo.Add("Action", nameof(GetClaims));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo}", scopeInfo);

            return Ok(_userService.Claims);
        }
    }
}