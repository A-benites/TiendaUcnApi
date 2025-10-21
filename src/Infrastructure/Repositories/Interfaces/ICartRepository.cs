using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces
{
    public interface ICartRepository
    {
        /// <summary>
        /// Busca un carrito por buyerId y opcionalmente por userId.
        /// </summary>
        Task<Cart?> FindAsync(string buyerId, int? userId);

        /// <summary>
        /// Obtiene un carrito por userId.
        /// </summary>
        Task<Cart?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Obtiene un carrito anónimo por buyerId.
        /// </summary>
        Task<Cart?> GetAnonymousAsync(string buyerId);

        /// <summary>
        /// Obtiene un carrito por buyerId (con o sin usuario asociado).
        /// </summary>
        Task<Cart?> GetByBuyerIdAsync(string buyerId);

        /// <summary>
        /// Crea un nuevo carrito.
        /// </summary>
        Task<Cart> CreateAsync(string buyerId, int? userId = null);

        /// <summary>
        /// Actualiza un carrito existente.
        /// </summary>
        Task UpdateAsync(Cart cart);

        /// <summary>
        /// Elimina un carrito.
        /// </summary>
        Task DeleteAsync(Cart cart);

        /// <summary>
        /// Agrega un item al carrito.
        /// </summary>
        Task AddItemAsync(Cart cart, CartItem cartItem);

        /// <summary>
        /// Elimina un item del carrito.
        /// </summary>
        Task RemoveItemAsync(CartItem cartItem);
    }
}
