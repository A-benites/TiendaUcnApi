using TiendaUcnApi.src.Application.DTO.CartDTO;


namespace TiendaUcnApi.src.Application.Services.Interfaces
{

    public interface ICartService
    {
        
        Task<CartDTO> AddItemAsync(string buyerId, int productId, int quantity, int? userId = null);

        Task<CartDTO> CreateOrGetAsync(string buyerId, int? userId = null);

        
        Task<CartDTO> RemoveItemAsync(string buyerId, int productId, int? userId = null);

        
        Task<CartDTO> ClearAsync(string buyerId, int? userId = null);

        
        Task<CartDTO> UpdateItemQuantityAsync(string buyerId, int productId, int quantity, int? userId = null);

        
        Task AssociateWithUserAsync(string buyerId, int userId);

        Task<CartDTO> CheckoutAsync(string buyerId, int? userId);
    }
}