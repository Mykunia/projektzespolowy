using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.ProductService
{
    public interface IProductService
    {
        Task GetProducts(string? categoryUrl = null);
        Task<ServiceResponse<Product>> GetProduct(int productId);
        Task SearchProducts(string searchText, int page);
        Task<List<string>> GetProductSearchSuggestions(string searchText);
        Task GetSecuredProducts();
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(Product product);
        event Action ProductsChanged;
        List<Product> Products { get; set; }
        List<Product> SecuredProducts { get; set; }
        string Message { get; set; }
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        string TextSearch { get; set; }
    }
}
