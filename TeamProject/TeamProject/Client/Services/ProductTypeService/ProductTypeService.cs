using System.Net.Http.Json;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly HttpClient _httpClient;
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        public event Action OnChange;
        public ProductTypeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddProductType(ProductType productType)
        {
            var response = await _httpClient.PostAsJsonAsync("api/producttype", productType);
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange.Invoke();
        }

        public async Task UpdateProductType(ProductType productType)
        {
            var response = await _httpClient.PutAsJsonAsync("api/producttype", productType);
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange.Invoke();
        }

        public ProductType CreateNewProductType()
        {
            var newPType = new ProductType
            {
                New = true,
                Edit = true
            };
            ProductTypes.Add(newPType);
            OnChange.Invoke();
            return newPType;
        }

        public async Task GetProductTypes()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/producttype");
            ProductTypes = response.Data;
        }

        public async Task DeleteProductType(int productTypeId)
        {
            var response = await _httpClient.DeleteAsync($"api/ProductType/{productTypeId}");
            ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
            OnChange?.Invoke();
        }
    }
}
