using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamProject.Server.Services.PaymentService;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("checkout")]
        [Authorize]
        public async Task<ActionResult<string>> CreateCheckOutSession()
        {
            var session = await _paymentService.CreateCheckoutSession();
            return Ok(session);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<bool>>> FullFillOrder()
        {
            var response = await _paymentService.FulfillOrder(Request);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
    }
}
