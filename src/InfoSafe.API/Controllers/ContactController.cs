using InfoSafe.ViewModel;
using InfoSafe.Write.Data.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SharedKernel.Enums;
using SharedKernel.Extensions;
using static InfoSafe.Read.Data.Queries.Queries;

namespace InfoSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IMediator _mediator;
        private readonly IFeatureManager _featureManager;

        public ContactController(
            ILogger<ContactController> logger,
            IMediator mediator,
            IFeatureManager featureManager)
        {
            _logger = logger;
            _mediator = mediator;
            _featureManager = featureManager;
        }

        [HttpGet()]
        public async Task<ActionResult> GetContacts()
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(ContactController));
            scopeInfo.Add("Action", nameof(GetContacts));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo}", scopeInfo);

            var query = new GetContactsQuery();
            var vm = await _mediator.Send(query);

            return Ok(vm);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetContactById(int id)
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(ContactController));
            scopeInfo.Add("Action", nameof(GetContactById));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, new { id });

            var query = new GetContactByIdQuery(id);
            var vm = await _mediator.Send(query);

            if (vm == null)
            {
                return NotFound();
            }

            return Ok(vm);
        }

        [HttpPost]
        public async Task<ActionResult> SaveContact([FromBody] ContactVM value)
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(ContactController));
            scopeInfo.Add("Action", nameof(SaveContact));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, value.OutputJson());

            var command = new ContactSaveCommand(value);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok() : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
        }

        [HttpGet]
        public async Task<ActionResult> GetFeatureTest()
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(ContactController));
            scopeInfo.Add("Action", nameof(GetFeatureTest));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo);

            var featureInfo = new Dictionary<string, object>();
            featureInfo.Add("FeatureA-Basic", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureA)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("FeatureB-Basic", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureB)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("FeatureC-Percentage", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureC)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("FeatureD-TimeWindow", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureD)) ? "Is enabled" : "Is not enabled");
            featureInfo.Add("FeatureE-Custom", await _featureManager.IsEnabledAsync(nameof(FeatureFlags.FeatureE)) ? "Is enabled" : "Is not enabled");
            return Ok(featureInfo);
        }
    }
}