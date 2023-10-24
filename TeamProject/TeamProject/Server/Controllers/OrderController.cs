using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamProject.Server.Services.OrderServices;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<OrderSummaryResponse>>>> GetOrders()
        {
            var response = await _orderService.GetOrders();
            return Ok(response);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<ServiceResponse<OrderDetailsResponse>>> GetOrderDetails(int orderId)
        {
            var response = await _orderService.GetOrderDetails(orderId);
            return Ok(response);
        }
    }
}
