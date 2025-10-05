using Microsoft.AspNetCore.Mvc;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Controlador base para herencia de otros controladores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase;