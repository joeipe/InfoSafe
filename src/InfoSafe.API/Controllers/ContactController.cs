using InfoSafe.ViewModel;
using InfoSafe.Write.Data.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Extensions;
using static InfoSafe.Read.Data.Queries.Queries;

namespace InfoSafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly IMediator _mediator;

        public ContactController(
            ILogger<ContactController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
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

        [HttpGet()]
        public async Task<ActionResult> GetFullContacts()
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(ContactController));
            scopeInfo.Add("Action", nameof(GetFullContacts));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo}", scopeInfo);

            var query = new GetFullContactsQuery();
            var vm = await _mediator.Send(query);

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
    }
}