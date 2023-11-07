using TeamProject.Shared.Models;

namespace TeamProject.Client.Services.CategoryService
{
    public interface ICategoryService
    {
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(int categoryId);
        Task GetSecuredCategories();
        Task GetCategories();
        Category CreateNewCategory();
        List<Category> Categories { get; set; }
        List<Category> SecuredCategories { get; set; }
        event Action OnChange;
    }
}
