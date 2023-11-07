using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.OrderService
{
    public interface IOrderService
    {
        Task<OrderDetailsResponse> GetOrderDetails(int orderId);
        Task<List<OrderSummaryResponse>> GetOrders();
        Task<string> PlaceOrder();
    }
}
