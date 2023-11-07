using System.Net.Http.Json;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Product> SecuredProducts { get; set; }
        public string Message { get; set; } = "Loading...";
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public string TextSearch { get; set; } = string.Empty;
        public event Action ProductsChanged;
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetProducts(string? categoryUrl = null)
        {
            var result = categoryUrl == null ?
                await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/featured") :
                await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
            if (result != null && result.Data != null)
                Products = result.Data;

            CurrentPage = 1;
            PageCount = 0;

            if (Products.Count == 0)
            {
                Message = "ProductsNotFound";
            }
            ProductsChanged.Invoke();

        }

        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{productId}");
            return response;
        }

        public async Task SearchProducts(string searchText, int page)
        {
            TextSearch = searchText;
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<ProductSearchResponse>>($"api/product/search/{searchText}/{page}");
            if (response != null && response.Data != null)
            {
                Products = response.Data.Products;
                CurrentPage = response.Data.CPages;
                PageCount = response.Data.Pages;
            }
            if (Products.Count == 0)
            {
                Message = "ProductNotFound";
            }
            ProductsChanged?.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
            return response.Data;
        }

        public async Task GetSecuredProducts()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product/admin");
            SecuredProducts = response.Data;
            CurrentPage = 1;
            PageCount = 0;
            if (SecuredProducts.Count == 0)
            {
                Message = "ProductsNotFound";
            }
        }

        public async Task<Product> CreateProduct(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync("api/product", product);
            var newProd = (await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>()).Data;
            return newProd;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/product", product);
            var req = await response.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
            return req.Data;
        }

        public async Task DeleteProduct(Product product)
        {
            var response = await _httpClient.DeleteAsync($"api/product/{product.Id}");
        }
    }
}
