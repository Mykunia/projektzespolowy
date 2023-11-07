using System.Net.Http.Json;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _httpClient;
        public AddressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Address> GetAddress()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<Address>>("api/address");
            return response.Data;
        }

        public async Task<Address> AddOrUpdateAddress(Address address)
        {
            var response = await _httpClient.PostAsJsonAsync("api/address", address);
            var result = response.Content.ReadFromJsonAsync<ServiceResponse<Address>>().Result.Data;

            return result;
        }
    }
}
