using TeamProject.Shared.Models;
using TeamProject.Shared.Response;

namespace TeamProject.Client.Services.CartService
{
    public interface ICartService
    {
        Task RemoveProductFromCart(int productId, int pTypeId);
        Task<List<CartProductResponse>> GetCartProducts();
        Task UpdateQuantity(CartProductResponse product);
        Task StoreCartItems(bool emptyCart);
        Task AddToCart(CartItem carItem);
        Task GetCountItems();

        event Action OnChange;
    }
}
