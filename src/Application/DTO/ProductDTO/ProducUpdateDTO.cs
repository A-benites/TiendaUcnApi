using System.ComponentModel.DataAnnotations;
using TiendaUcnApi.src.Domain.Models;

namespace TiendaUcnApi.src.Application.DTO.ProductDTO;

public class ProducUpdateDTO
{
    [StringLength(50, MinimumLength = 3)]
    public string? Title { get; set; }

    [MinLength(10)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int? Price { get; set; }

    [Range(0, 100)]
    public int? Discount { get; set; }

    [Range(0, int.MaxValue)]
    public int? Stock { get; set; }

    public Status? Status { get; set; }
}
