using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.BaseResponse;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Retorna todos los productos para el administrador según los parámetros de búsqueda.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
    /// <returns>Una lista de productos filtrados para el administrador.</returns>
    Task<ListedProductsForAdminDTO> GetFilteredForAdminAsync(SearchParamsDTO searchParams);

    /// <summary>
    /// Retorna todos los productos para el cliente según los parámetros de búsqueda.
    /// </summary>
    /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
    /// <returns>Una lista de productos filtrados para el cliente.</returns>
    Task<GenericResponse<object>> GetFilteredForCustomerAsync(SearchParamsDTO searchParams);

    /// <summary>
    /// Retorna un producto específico por su ID.
    /// </summary>
    /// <param name="id">El ID del producto a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
    Task<ProductDetailDTO> GetByIdAsync(int id);

    /// <summary>
    /// Retorna un producto específico por su ID desde el punto de vista de un admin.
    /// </summary>
    /// <param name="id">El ID del producto a buscar.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
    Task<ProductDetailDTO> GetByIdForAdminAsync(int id);

    /// <summary>
    /// Retorna un producto específico por su ID para el cliente.
    /// </summary>
    /// <param name="id">El ID del producto a buscar.</param>
    /// <returns>Respuesta con el producto encontrado.</returns>
    Task<GenericResponse<ProductDetailDTO>> GetByIdForCustomerAsync(int id);

    /// <summary>
    /// Crea un nuevo producto en el sistema.
    /// </summary>
    /// <param name="createProductDTO">Los datos del producto a crear.</param>
    /// <returns>Una tarea que representa la operación asíncrona, con el id del producto creado.</returns>
    Task<string> CreateAsync(ProductCreateDTO createProductDTO);

    /// <summary>
    /// Cambia el estado activo de un producto por su ID.
    /// </summary>
    /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
    Task ToggleActiveAsync(int id);

    Task<ProductDetailDTO> UpdateAsync(int id, ProducUpdateDTO producUpdateDTO);

    Task UpdateDiscountAsync(int id, UpdateProductDiscountDTO dto);
}
