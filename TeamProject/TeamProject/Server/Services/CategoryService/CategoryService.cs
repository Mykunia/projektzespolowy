using Microsoft.EntityFrameworkCore;
using TeamProject.Server.Data;
using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private async Task<Category> GetCategoryById(int id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServiceResponse<List<Category>>> AddCategory(Category category)
        {
            category.Edit = category.New = false;
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return await GetSecuredCategories();
        }

        public async Task<ServiceResponse<List<Category>>> DeleteCategory(int id)
        {
            Category category = await GetCategoryById(id);
            if(category == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "implement this"
                };
            }
            category.Deleted = true;
            await _dbContext.SaveChangesAsync();

            return await GetSecuredCategories();
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categories = await _dbContext.Categories
                .Where(c => !c.Deleted && c.Visibility)
                .ToListAsync();
            return new ServiceResponse<List<Category>> { Data = categories };
        }

        public async Task<ServiceResponse<List<Category>>> GetSecuredCategories()
        {
            var categories = await _dbContext.Categories
                .Where(c => !c.Deleted)
                .ToListAsync();
            return new ServiceResponse<List<Category>> { Data = categories };
        }

        public async Task<ServiceResponse<List<Category>>> UpdateCategory(Category category)
        {
            var getCategory = await GetCategoryById(category.Id);
            if(getCategory == null)
            {
                return new ServiceResponse<List<Category>>
                {
                    Success = false,
                    Message = "Idk"
                };
            }

            getCategory.Name = category.Name;
            getCategory.Url = category.Url;
            getCategory.Visibility = category.Visibility;

            await _dbContext.SaveChangesAsync();
            return await GetSecuredCategories();
        }
    }
}
