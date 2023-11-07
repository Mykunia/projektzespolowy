using Blazored.LocalStorage;
using System.Net.Http.Json;
using TeamProject.Client.Services.AuthService;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        public CartService(ILocalStorageService localStorageService, HttpClient httpClient, IAuthService authService)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
            _authService = authService;
        }

        public event Action OnChange;

        public async Task AddToCart(CartItem carItem)
        {
            if (await _authService.IsUserAuth())
            {
                await _httpClient.PostAsJsonAsync("api/cart/add", carItem);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                var items = cart.Find(x => x.ProductId == carItem.ProductId &&
                    x.ProductTypeId == carItem.ProductTypeId);
                if (items == null)
                {
                    cart.Add(carItem);
                }
                else
                {
                    items.Quantity += carItem.Quantity;
                }

                await _localStorageService.SetItemAsync("cart", cart);
            }
            await GetCountItems();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            if (await _authService.IsUserAuth())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
                return response.Data;
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return new List<CartProductResponse>();
                }
                var response = await _httpClient.PostAsJsonAsync("api/cart/products", cart);
                var cartItems = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
                return cartItems.Data;
            }
        }

        public async Task GetCountItems()
        {
            if (await _authService.IsUserAuth())
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = response.Data;
            }
            else
            {
                var car = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                await _localStorageService.SetItemAsync<int>("cartItemsCount", car != null ? car.Count : 0);
            }
            OnChange.Invoke();
        }

        public async Task RemoveProductFromCart(int productId, int pTypeId)
        {
            if (await _authService.IsUserAuth())
            {
                await _httpClient.DeleteAsync($"api/cart/{productId}/{pTypeId}");
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }
                var items = cart.Find(x => x.ProductId == productId && x.ProductTypeId == pTypeId);
                if (items != null)
                {
                    cart.Remove(items);
                    await _localStorageService.SetItemAsync("cart", cart);
                }
            }
        }

        public async Task StoreCartItems(bool emptyCart)
        {
            var lCart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
            if (lCart == null)
            {
                return;
            }
            await _httpClient.PostAsJsonAsync("api/cart", lCart);
            if (emptyCart)
            {
                await _localStorageService.RemoveItemAsync("cart");
            }
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            if (await _authService.IsUserAuth())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductTypeId = product.ProductTypeId
                };
                await _httpClient.PutAsJsonAsync("api/cart/update-quantity", request);
            }
            else
            {
                var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
                if (cart == null)
                {
                    return;
                }
                var items = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);
                if (items != null)
                {
                    items.Quantity = product.Quantity;
                    await _localStorageService.SetItemAsync("cart", cart);
                }
            }
        }
    }
}
