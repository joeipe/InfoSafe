using InfoSafe.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                    Amount = paymentRequest.Amount * 100,
                    Currency = "aud",
                    //AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    //{
                    //    Enabled = true,
                    //},
                    PaymentMethodTypes = new List<string> { "card" }
                };
                var service = new PaymentIntentService();
                paymentIntent = await service.CreateAsync(options);
                var response = new PaymentResponse
                {
                    ClientSecret = paymentIntent.ClientSecret
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}