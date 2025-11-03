using Microsoft.AspNetCore.Mvc;

namespace TiendaUcnApi.src.API.Controllers;

/// <summary>
/// Base controller for inheritance by other API controllers.
/// Provides common functionality and conventions for all controllers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase;
