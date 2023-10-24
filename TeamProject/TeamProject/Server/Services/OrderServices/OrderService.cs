using Microsoft.EntityFrameworkCore;
using TeamProject.Server.Data;
using TeamProject.Server.Services.AuthService;
using TeamProject.Server.Services.CartService;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _dbContext;
        public OrderService(ICartService cartService, IAuthService authService, ApplicationDbContext dbContext)
        {
            _cartService = cartService;
            _authService = authService;
            _dbContext = dbContext;
        }
        public async Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId)
        {
            var response = new ServiceResponse<OrderDetailsResponse>();
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductType)
                .Where(o => o.UserId == _authService.GetUserId() && o.Id == orderId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                response.Success = false;
                response.Message = "OrderNotFound";
                return response;
            }

            var orderDetailsResponse = new OrderDetailsResponse
            {
                OrderDate = order.OrderDate,
                TotalPrice = order.Total,
                Products = new List<OrderDetailsProductResponse>()
            };

            order.OrderItems.ForEach(item =>
            orderDetailsResponse.Products.Add(new OrderDetailsProductResponse
            {
                ProductId = item.ProductId,
                PictureUrl = item.Product.PictureUrl,
                ProductType = item.ProductType.Name,
                Quantity = item.Quantity,
                Name = item.Product.Name,
                TotalPrice = item.TotalPrice
            }));

            response.Data = orderDetailsResponse;

            return response;
        }

        public async Task<ServiceResponse<List<OrderSummaryResponse>>> GetOrders()
        {
            var response = new ServiceResponse<List<OrderSummaryResponse>>();
            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == _authService.GetUserId())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderResponse = new List<OrderSummaryResponse>();
            orders.ForEach(o => orderResponse.Add(new OrderSummaryResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.Total,
                Product = o.OrderItems.Count > 1 ?
                    $"{o.OrderItems.First().Product.Name} and" +
                    $" {o.OrderItems.Count - 1} more..." :
                    o.OrderItems.First().Product.Name,
                ProductImageUrl = o.OrderItems.First().Product.PictureUrl
            }));

            response.Data = orderResponse;

            return response;
        }

        public async Task<ServiceResponse<bool>> PlaceOrder(int userId)
        {
            var products = (await _cartService.GetDbCartProducts(userId)).Data;
            decimal totalPrice = 0;
            products.ForEach(product => totalPrice += product.Price * product.Quantity);

            var orderItems = new List<OrderItem>();
            products.ForEach(product => orderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Price * product.Quantity
            }));

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Total = totalPrice,
                OrderItems = orderItems
            };

            _dbContext.Orders.Add(order);

            _dbContext.CartItems.RemoveRange(_dbContext.CartItems
                .Where(ci => ci.UserId == userId));

            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }
    }
}
