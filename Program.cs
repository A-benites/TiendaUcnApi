using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using Serilog;
using System.Security.Claims;
using System.Text;
using TiendaUcnApi.src.API.Extensions;
using TiendaUcnApi.src.API.Middlewares.ErrorHandlingMiddleware;
using TiendaUcnApi.src.Application.Mappers; // <-- Importación añadida
using TiendaUcnApi.src.Application.Services.Implements;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Implements;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

// Configura un logger de arranque para capturar errores durante el inicio.
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

Log.Information("Iniciando la aplicación");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Logging Configuration
    // Reemplaza el logger por defecto de .NET con Serilog.
    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );
    #endregion

    #region Database Configuration
    Log.Information("Configurando base de datos SQLite");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
    #endregion

    #region Identity Configuration
    Log.Information("Configurando Identity");
    // Se usa AddIdentityCore para un control más fino.
    builder
        .Services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;

            // Habilitar la validación del SecurityStamp
            // options.SecurityStampValidationInterval = TimeSpan.Zero; // Esta opción no existe en IdentityOptions
        })
        .AddRoles<Role>() // Asegúrate de tener un modelo Role
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    #endregion

    #region Authentication Configuration
    Log.Information("Configurando autenticación JWT");
    builder
        .Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            string jwtSecret =
                builder.Configuration["JWTSecret"]
                ?? throw new InvalidOperationException("La clave secreta JWT no está configurada.");
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // No hay tolerancia para tokens expirados
            };

            // --- CÓDIGO AÑADIDO PARA VALIDAR EL SECURITY STAMP (R35) ---
            options.Events = new JwtBearerEvents
            {
                // Este evento se ejecuta después de validar el token, pero antes de que se establezca la identidad del usuario.
                OnTokenValidated = async context =>
                {
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<
                        UserManager<User>
                    >();
                    var claimsPrincipal = context.Principal;
                    if (claimsPrincipal == null)
                    {
                        context.Fail("Token inválido.");
                        return;
                    }

                    var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await userManager.FindByIdAsync(userId);

                    // Compara el SecurityStamp del token con el de la base de datos.
                    // Si no coinciden, significa que la contraseña cambió (u otra acción de seguridad) y el token ya no es válido.
                    if (
                        user == null
                        || user.SecurityStamp
                            != claimsPrincipal.FindFirstValue("AspNet.Identity.SecurityStamp")
                    )
                    {
                        context.Fail("Token inválido. Sesión expirada por cambio de seguridad.");
                    }
                },
            };
        });
    #endregion

    #region Dependency Injection
    Log.Information("Configurando inyección de dependencias");
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<ITokenService, TokenService>(); // Registro del servicio de token
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<IFileRepository, FileRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<ICartRepository, CartRepository>();
    builder.Services.AddScoped<ICartService, CartService>();

    #endregion

    #region Email Service Configuration
    Log.Information("Configurando servicio de Email");
    builder.Services.AddOptions();
    builder.Services.AddHttpClient<ResendClient>();
    builder.Services.Configure<ResendClientOptions>(o =>
    {
        o.ApiToken =
            builder.Configuration["ResendAPIKey"]
            ?? throw new InvalidOperationException(
                "El token de API de Resend no está configurado."
            );
    });
    builder.Services.AddTransient<IResend, ResendClient>();
    #endregion

    // Configura los controladores y el comportamiento de la API
    builder
        .Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context
                    .ModelState.Where(kvp => kvp.Value != null && kvp.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp =>
                            kvp.Value!.Errors.Select(e => e.ErrorMessage ?? string.Empty).ToArray()
                    );

                return new BadRequestObjectResult(
                    new
                    {
                        status = 400,
                        message = "Errores de validación",
                        errors,
                        timestamp = DateTime.UtcNow,
                    }
                );
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    // Configura los mapeos de Mapster
    var userMapper = new UserMapper();
    userMapper.ConfigureAllMappings();

    var productMapper = new ProductMapper(builder.Configuration); // <-- Línea añadida
    productMapper.ConfigureAllMappings(); // <-- Línea añadida

    var app = builder.Build();

    #region Pipeline Configuration
    Log.Information("Configurando el pipeline de la aplicación");

    // Middleware para logs de requests
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tienda UCN API V1");
            c.RoutePrefix = string.Empty; // Swagger en la raíz
        });
    }

    // Middleware personalizado para manejo de excepciones
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<TiendaUcnApi.src.API.Middleware.BuyerIdMiddleware>();


    app.UseHttpsRedirection();

    // IMPORTANTE: El orden de UseAuthentication y UseAuthorization es crucial
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Aplica migraciones y datos semilla al iniciar
    await app.SeedDatabaseAsync();
    #endregion

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
}
finally
{
    Log.Information("Apagando la aplicación");
    Log.CloseAndFlush();
}