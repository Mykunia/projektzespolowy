using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly AuthenticationStateProvider _stateProvider;
        private readonly NavigationManager _navManager;
        private readonly HttpClient _httpClient;
        public OrderService(AuthenticationStateProvider stateProvider, NavigationManager navManager, HttpClient httpClient)
        {
            _stateProvider = stateProvider;
            _navManager = navManager;
            _httpClient = httpClient;
        }

        private async Task<bool> UserAuth()
        {
            return (await _stateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }
        public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<OrderDetailsResponse>>($"api/order/{orderId}");
            return response.Data;
        }

        public async Task<List<OrderSummaryResponse>> GetOrders()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<OrderSummaryResponse>>>("api/order");
            return response.Data;
        }

        public async Task<string> PlaceOrder()
        {
            if (await UserAuth())
            {
                var response = await _httpClient.PostAsync("api/payment/checkout", null);
                var address = await response.Content.ReadAsStringAsync();
                return address;
            }
            else
            {
                return "login";
            }
        }
    }
}
