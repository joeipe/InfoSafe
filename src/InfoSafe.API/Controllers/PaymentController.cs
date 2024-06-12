using CSharpFunctionalExtensions;
using InfoSafe.ViewModel;
using InfoSafe.Write.Data.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Extensions;
using Stripe;

namespace InfoSafe.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentController(
            ILogger<PaymentController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> CreatePaymentIntent([FromBody] PaymentRequest paymentRequest)
        {
            PaymentIntent paymentIntent;

            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(PaymentController));
            scopeInfo.Add("Action", nameof(CreatePaymentIntent));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, paymentRequest.Amount);

            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:ApiKey"];
                var options = new PaymentIntentCreateOptions
                {
                    Amount = paymentRequest.Amount,
                    Currency = "aud",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                };
                var service = new PaymentIntentService();
                paymentIntent = service.Create(options);
                return Ok(paymentIntent);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
