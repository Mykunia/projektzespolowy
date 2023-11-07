using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using TeamProject.Shared.Response;
using TeamProject.Shared.User;

namespace TeamProject.Client.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AuthenticationStateProvider _stateProvider;
        private readonly HttpClient _httpClient;
        public AuthService(AuthenticationStateProvider stateProvider, HttpClient httpClient)
        {
            _stateProvider = stateProvider;
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword req)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", req.Password);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<bool> IsUserAuth()
        {
            return (await _stateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public async Task<ServiceResponse<string>> Login(UserLogin req)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", req);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        }

        public async Task<ServiceResponse<int>> Register(UserRegister req)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", req);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }
    }
}
