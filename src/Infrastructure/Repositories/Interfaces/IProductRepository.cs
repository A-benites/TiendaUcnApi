using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Interfaz para el repositorio de productos, que define los métodos para interactuar con los datos de productos.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retorna una lista de productos para el administrador con los parámetros de búsqueda especificados.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el administrador y el conteo total de productos.</returns>
    Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForAdminAsync(
        SearchParamsDTO searchParams
    );

    /// <summary>
    /// Retorna una lista de productos para el cliente con los parámetros de búsqueda especificados.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con una lista de productos para el cliente y el conteo total de productos.</returns>
    Task<(IEnumerable<Product> products, int totalCount)> GetFilteredForCustomerAsync(
        SearchParamsDTO searchParams
    );

    /// <summary>
    /// Retorna un producto específico por su ID.
    /// </summary>
    /// <param name="id">El ID del producto a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// Crea un nuevo producto en el repositorio.
    /// </summary>
    /// <param name="product">El producto a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el id del producto creado.</returns>
    Task<int> CreateAsync(Product product);

    /// <summary>
    /// Obtiene una categoría por su Id.
    /// </summary>
    /// <param name="id">Id de la categoría.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con la categoría encontrada.</returns>
    Task<Category?> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Obtiene una marca por su Id.
    /// </summary>
    /// <param name="id">Id de la marca.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con la marca encontrada.</returns>
    Task<Brand?> GetBrandByIdAsync(int id);

    /// <summary>
    /// Cambia el estado activo de un producto por su ID.
    /// </summary>
    /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
    Task ToggleActiveAsync(int id);

    /// <summary>
    /// Obtiene el stock real de un producto por su ID.
    /// </summary>
    /// <param name="productId">El ID del producto cuyo stock se obtendrá.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el stock real del producto.</returns>
    Task<int> GetRealStockAsync(int productId);

    /// <summary>
    /// Actualiza el stock de un producto por su ID.
    /// </summary>
    /// <param name="productId">El ID del producto cuyo stock se actualizará.</param>
    /// <param name="stock">El nuevo stock del producto.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateStockAsync(int productId, int stock);

    /// <summary>
    /// Retorna un producto específico por su ID desde el punto de vista de un admin.
    /// </summary>
    /// <param name="id">El ID del producto a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
    Task<Product?> GetByIdForAdminAsync(int id);

    /// <summary>
    /// Actualiza los datos de un producto.
    /// </summary>
    /// <param name="product">Producto con los datos actualizados.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el producto actualizado.</returns>
    Task<Product> UpdateAsync(Product product);

    /// <summary>
    /// Actualiza el descuento de un producto.
    /// </summary>
    /// <param name="productId">ID del producto.</param>
    /// <param name="discount">Nuevo descuento.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateDiscountAsync(int productId, int discount);

    /// <summary>
    /// Obtiene un producto con seguimiento de cambios por su ID para administración.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no existe.</returns>
    Task<Product?> GetTrackedByIdForAdminAsync(int id);
}