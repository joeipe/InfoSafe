using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SharedKernel.Enums;

namespace InfoSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FeatureController : ControllerBase
    {
        private readonly ILogger<FeatureController> _logger;
        private readonly IFeatureManager _featureManager;

        public FeatureController(
            ILogger<FeatureController> logger,
            IFeatureManager featureManager)
        {
            _logger = logger;
            _featureManager = featureManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetFeatureTest()
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(ContactController));
            scopeInfo.Add("Action", nameof(GetFeatureTest));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo}", scopeInfo);

            var featureInfo = new Dictionary<string, object>();
            featureInfo.Add("featureA_basic", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureA)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("featureB_basic", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureB)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("featureC_percentage", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureC)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("featureD_timeWindow", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureD)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("featureE_custom", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureE)) ? "Is enabled" : "Is not enabled");
            return Ok(featureInfo);
        }
    }
}