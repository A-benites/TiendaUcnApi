namespace TiendaUcnApi.src.Application.DTO.BrandDTO;

public class BrandDTO
{
    
    public int Id { get; set; }

    
    public required string Name { get; set; }

    
    public DateTime CreatedAt { get; set; }

    
    public int ProductCount { get; set; }
}
