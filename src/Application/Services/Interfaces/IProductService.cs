using TiendaUcnApi.src.Application.DTO;
using TiendaUcnApi.src.Application.DTO.BaseResponse;
using TiendaUcnApi.src.Application.DTO.ProductDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUcnApi.src.Application.DTO.ProductDTO.ConsumerDTO;

namespace TiendaUcnApi.src.Application.Services.Interfaces;

/// <summary>
/// Service interface for product management operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Retrieves all products for admin view with filtering and pagination.
    /// </summary>
    /// <param name="searchParams">Search parameters for filtering products.</param>
    /// <returns>A list of filtered products for admin with full details.</returns>
    Task<ListedProductsForAdminDTO> GetFilteredForAdminAsync(SearchParamsDTO searchParams);

    /// <summary>
    /// Retrieves all products for customer view with filtering and pagination.
    /// Only returns available products.
    /// </summary>
    /// <param name="searchParams">Search parameters for filtering products.</param>
    /// <returns>A list of filtered products for customers.</returns>
    Task<GenericResponse<object>> GetFilteredForCustomerAsync(SearchParamsDTO searchParams);

    /// <summary>
    /// Retrieves a specific product by its ID (internal use).
    /// </summary>
    /// <param name="id">The product ID to search for.</param>
    /// <returns>Product details or null if not found.</returns>
    Task<ProductDetailDTO> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a specific product by its ID for admin view.
    /// Includes all product details regardless of availability status.
    /// </summary>
    /// <param name="id">The product ID to search for.</param>
    /// <returns>Product details including admin-specific information.</returns>
    Task<ProductDetailForAdminDTO> GetByIdForAdminAsync(int id);

    /// <summary>
    /// Retrieves a specific product by its ID for customer view.
    /// Only returns the product if it's available.
    /// </summary>
    /// <param name="id">The product ID to search for.</param>
    /// <returns>Response with product details if available.</returns>
    Task<GenericResponse<ProductDetailDTO>> GetByIdForCustomerAsync(int id);

    /// <summary>
    /// Creates a new product in the system with images.
    /// </summary>
    /// <param name="createProductDTO">Product data including images to create.</param>
    /// <returns>Success message with the created product ID.</returns>
    Task<string> CreateAsync(ProductCreateDTO createProductDTO);

    /// <summary>
    /// Toggles the availability status of a product (soft delete).
    /// </summary>
    /// <param name="id">The product ID whose status will be toggled.</param>
    Task ToggleActiveAsync(int id);

    /// <summary>
    /// Updates an existing product's information.
    /// </summary>
    /// <param name="id">The product ID to update.</param>
    /// <param name="producUpdateDTO">DTO containing updated product data.</param>
    /// <returns>Updated product details.</returns>
    Task<ProductDetailDTO> UpdateAsync(int id, ProducUpdateDTO producUpdateDTO);

    /// <summary>
    /// Updates the discount percentage of a product.
    /// </summary>
    /// <param name="id">The product ID to update.</param>
    /// <param name="dto">DTO containing the new discount percentage.</param>
    Task UpdateDiscountAsync(int id, UpdateProductDiscountDTO dto);
}
