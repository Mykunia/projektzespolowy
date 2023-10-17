using System.Collections.Generic;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.ProductTypeService
{
    public interface IProductTypeService
    {
        Task<ServiceResponse<List<ProductType>>> GetAllProductTypes();
        Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType);
        Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType);
        Task<ServiceResponse<List<ProductType>>> DeleteProductType(int producTypeId);
    }
}
