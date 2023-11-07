using TeamProject.Shared.Models;

namespace TeamProject.Client.Services.ProductTypeService
{
    public interface IProductTypeService
    {
        Task AddProductType(ProductType productType);
        Task UpdateProductType(ProductType productType);
        ProductType CreateNewProductType();
        Task GetProductTypes();
        Task DeleteProductType(int productTypeId);
        public List<ProductType> ProductTypes { get; set; }

        event Action OnChange;
    }
}
