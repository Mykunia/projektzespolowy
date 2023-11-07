using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeamProject.Server.Data;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<Product>> CreateProduct(Product product)
        {
            foreach(var item in product.Variants)
            {
                item.ProductType = null;
            }

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<Product>
            { 
                Data = product 
            };
        }

        public async Task<ServiceResponse<bool>> DeleteProduct(int productId)
        {
            var result = await _dbContext.Products.FindAsync(productId);
            if(result == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = " "
                };
            }

            result.Deleted = true;
            await _dbContext.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };

        }

        public async Task<ServiceResponse<List<Product>>> GetAdminProducts()
        {
            var result = new ServiceResponse<List<Product>>
            {
                Data = await _dbContext.Products
                    .Where(p => !p.Deleted)
                    .Include(p => p.Variants.Where(v => !v.Deleted))
                    .ThenInclude(v => v.ProductType)
                    .Include(p => p.Pictures)
                    .ToListAsync()
            };

            return result;
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _dbContext.Products
                .Where(p => !p.Deleted)
                .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.ProductType)
                .Include(p => p.Pictures)
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            Product product = null;

            if (_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                product = await _dbContext.Products
                    .Include(p => p.Variants.Where(v => !v.Deleted))
                    .ThenInclude(v => v.ProductType)
                    .Include(p => p.Pictures)
                    .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted);
            }
            else
            {
                product = await _dbContext.Products
                    .Include(p => p.Variants.Where(v => v.Visibility && !v.Deleted))
                    .ThenInclude(v => v.ProductType)
                    .Include(p => p.Pictures)
                    .FirstOrDefaultAsync(p => p.Id == productId && !p.Deleted && p.Visibility);
            }

            if (product == null)
            {
                response.Success = false;
                response.Message = " ";
            }
            else
            {
                response.Data = product;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _dbContext.Products
                .Where(p => p.Visibility && !p.Deleted)
                .Include(p => p.Variants.Where(v => v.Visibility && !v.Deleted))
                .Include(p => p.Pictures)
                .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _dbContext.Products
                    .Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower()) &&
                    p.Visibility && !p.Deleted)
                    .Include(p => p.Variants.Where(v => v.Visibility && !v.Deleted))
                    .Include(p => p.Pictures)
                    .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if (product.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Name);
                }

                if (product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                        .Distinct().ToArray();
                    var words = product.Description.Split()
                        .Select(s => s.Trim(punctuation));

                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<ProductSearchResponse>> SearchProducts(string searchText, int page)
        {
            var pageResult = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count() / pageResult);
            var products = await _dbContext.Products
                                .Where(p => p.Name.ToLower().Contains(searchText.ToLower()) ||
                                    p.Description.ToLower().Contains(searchText.ToLower()) &&
                                    p.Visibility && !p.Deleted)
                                .Include(p => p.Variants)
                                .Include(p => p.Pictures)
                                .Skip((page - 1) * (int)pageResult)
                                .Take((int)pageResult)
                                .ToListAsync();
            var response = new ServiceResponse<ProductSearchResponse>
            {
                Data = new ProductSearchResponse
                {
                    Products = products,
                    CPages = page,
                    Pages = (int)pageCount
                }
            };

            return response;
        }

        public async Task<ServiceResponse<Product>> UpdateProduct(Product product)
        {
            var getProduct = await _dbContext.Products
                .Include(p => p.Pictures)
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (getProduct == null)
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = " "
                };
            }

            getProduct.Name = product.Name;
            getProduct.Description = product.Description;
            getProduct.PictureUrl = product.PictureUrl;
            getProduct.CategoryId = product.CategoryId;
            getProduct.Visibility = product.Visibility;
            getProduct.Featured = product.Featured;

            var productImages = getProduct.Pictures;
            _dbContext.Pictures.RemoveRange(productImages);

            getProduct.Pictures = product.Pictures;

            foreach (var variant in product.Variants)
            {
                var getVariant = await _dbContext.ProductVariants
                    .SingleOrDefaultAsync(v => v.ProductId == variant.ProductId &&
                        v.ProductTypeId == variant.ProductTypeId);
                if (getVariant == null)
                {
                    variant.ProductType = null;
                    _dbContext.ProductVariants.Add(variant);
                }
                else
                {
                    getVariant.ProductTypeId = variant.ProductTypeId;
                    getVariant.Price = variant.Price;
                    getVariant.OriginalPrice = variant.OriginalPrice;
                    getVariant.Visibility = variant.Visibility;
                    getVariant.Deleted = variant.Deleted;
                }
            }

            await _dbContext.SaveChangesAsync();
            return new ServiceResponse<Product> { Data = product };
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _dbContext.Products
                                .Where(p => p.Name.ToLower().Contains(searchText.ToLower()) ||
                                    p.Description.ToLower().Contains(searchText.ToLower()) &&
                                    p.Visibility && !p.Deleted)
                                .Include(p => p.Variants)
                                .ToListAsync();
        }
    }
}
