using Microsoft.EntityFrameworkCore;
using TeamProject.Server.Data;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.ProductTypeService
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly ApplicationDbContext _dbContext;
        public ProductTypeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ServiceResponse<List<ProductType>>> AddProductType(ProductType productType)
        {
            var response = new ServiceResponse<List<ProductType>>();
            try
            {
                productType.Edit = productType.New = false;
                _dbContext.ProductTypes.Add(productType);
                await _dbContext.SaveChangesAsync();

                return await GetAllProductTypes();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<ProductType>>> DeleteProductType(int producTypeId)
        {
            var productType = await _dbContext.ProductTypes.FindAsync(producTypeId);

            if (productType == null)
            {
                return new ServiceResponse<List<ProductType>>
                {
                    Success = false,
                    Message = "ProductNotFound"
                };
            }

            _dbContext.ProductTypes.Remove(productType);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<List<ProductType>>
            {
                Data = await _dbContext.ProductTypes.ToListAsync()
            };
        }

        public async Task<ServiceResponse<List<ProductType>>> GetAllProductTypes()
        {
            var response = new ServiceResponse<List<ProductType>>();
            try
            {
                var productTypes = await _dbContext.ProductTypes.ToListAsync();
                return new ServiceResponse<List<ProductType>> { Data = productTypes };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<ProductType>>> UpdateProductType(ProductType productType)
        {
            var response = new ServiceResponse<List<ProductType>>();
            try
            {
                var getPType = await _dbContext.ProductTypes.FindAsync(productType.Id);
                if (getPType == null)
                {
                    return new ServiceResponse<List<ProductType>>
                    {
                        Success = false,
                        Message = "ProductNotFound"
                    };
                }

                getPType.Name = productType.Name;
                await _dbContext.SaveChangesAsync();

                return await GetAllProductTypes();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
