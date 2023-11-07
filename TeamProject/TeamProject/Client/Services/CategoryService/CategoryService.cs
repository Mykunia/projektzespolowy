using System.Net.Http.Json;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Category> SecuredCategories { get; set; } = new List<Category>();
        public event Action OnChange;
        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddCategory(Category category)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Category/admin", category);
            SecuredCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public async Task UpdateCategory(Category category)
        {
            var response = await _httpClient.PutAsJsonAsync("api/Category/admin", category);
            SecuredCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public async Task DeleteCategory(int categoryId)
        {
            var response = await _httpClient.DeleteAsync($"api/Category/admin/{categoryId}");
            SecuredCategories = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<Category>>>()).Data;
            await GetCategories();
            OnChange.Invoke();
        }

        public async Task GetSecuredCategories()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("/api/Category/admin");
            if (response != null && response.Data != null)
            {
                SecuredCategories = response.Data;
            }
        }

        public async Task GetCategories()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Category>>>("api/Category");
            if (response != null && response.Data != null)
                Categories = response.Data;
        }

        public Category CreateNewCategory()
        {
            var newCat = new Category { New = true, Edit = true };
            SecuredCategories.Add(newCat);
            OnChange.Invoke();
            return newCat;
        }
    }
}
