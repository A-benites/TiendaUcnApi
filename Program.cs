using System.Security.Claims;
using System.Text;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using Serilog;
using TiendaUcnApi.src.API.Extensions;
using TiendaUcnApi.src.API.Middlewares.ErrorHandlingMiddleware;
using TiendaUcnApi.src.Application.Mappers;
using TiendaUcnApi.src.Application.Services.Implements;
using TiendaUcnApi.src.Application.Services.Interfaces;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;
using TiendaUcnApi.src.Infrastructure.Repositories.Implements;
using TiendaUcnApi.src.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Main entry point for the TiendaUcnApi application.
/// Configures and initializes all services, middleware, authentication, and background jobs.
/// </summary>
// Bootstrap logger - Creates a minimal logger for startup diagnostics
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Logging Configuration
    // Configure Serilog for structured logging
    // Reads configuration from appsettings.json and enriches logs with contextual information
    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );
    #endregion

    #region Database Configuration
    // Configure SQLite database with Entity Framework Core
    Log.Information("Configuring SQLite database");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
    #endregion

    #region Hangfire Configuration
    // Configure Hangfire for background job processing
    // Uses SQLite storage to persist job state and schedules
    Log.Information("Configuring Hangfire with SQLite storage");

    builder.Services.AddHangfire(config =>
        config
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSQLiteStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    builder.Services.AddHangfireServer();
    #endregion

    #region Identity Configuration
    // Configure ASP.NET Core Identity for user management
    // Password requirements: 8+ chars, at least one digit
    // No special chars, uppercase, or lowercase requirements for better UX
    Log.Information("Configuring Identity");
    builder
        .Services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
    #endregion

    #region Authentication Configuration
    // Configure JWT Bearer authentication
    // Validates tokens on each request and checks security stamp for session invalidation
    Log.Information("Configuring JWT authentication");
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
                ?? throw new InvalidOperationException("JWT secret key not configured.");

            // Preserve standard JWT claim names (avoid mapping to Microsoft claim names)
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // No tolerance for expired tokens
            };

            // Custom token validation event to verify security stamp
            // Implements R115, R117: Invalidates sessions when user status/role changes
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<
                        UserManager<User>
                    >();
                    var claimsPrincipal = context.Principal;

                    if (claimsPrincipal == null)
                    {
                        context.Fail("Invalid token.");
                        return;
                    }

                    var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Fail("Invalid token. User ID not found.");
                        return;
                    }

                    var user = await userManager.FindByIdAsync(userId);

                    // Validate security stamp to detect if user was blocked or role changed
                    if (
                        user == null
                        || user.SecurityStamp
                            != claimsPrincipal.FindFirstValue("AspNet.Identity.SecurityStamp")
                    )
                    {
                        context.Fail("Invalid token. Session expired due to security change.");
                    }
                },
            };
        });
    #endregion

    #region Dependency Injection
    // Register all application services and repositories
    // Scoped lifetime: one instance per HTTP request
    Log.Information("Configuring dependency injection");

    // User & Authentication Services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<IUserAdminService, UserAdminService>();

    // Product & Catalog Services
    builder.Services.AddScoped<IFileRepository, FileRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IBrandRepository, BrandRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IBrandService, BrandService>();

    // Shopping & Order Services
    builder.Services.AddScoped<ICartRepository, CartRepository>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    // Background Job Service
    builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();
    #endregion

    #region Email Service Configuration
    // Configure Resend email service
    // Requires ResendAPIKey in configuration (appsettings.json or environment variables)
    Log.Information("Configuring email service");
    builder.Services.AddOptions();
    builder.Services.AddHttpClient<ResendClient>();
    builder.Services.Configure<ResendClientOptions>(o =>
    {
        o.ApiToken =
            builder.Configuration["ResendAPIKey"]
            ?? throw new InvalidOperationException("Resend API key not configured.");
    });
    builder.Services.AddTransient<IResend, ResendClient>();
    #endregion

    // Configure API controllers with custom validation error formatting
    builder
        .Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            // Custom validation error response format
            // Returns structured JSON with field-level errors
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
                        message = "Validation errors",
                        errors,
                        timestamp = DateTime.UtcNow,
                    }
                );
            };
        });

    // Configure API documentation and supporting services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    #region Mapster Configuration
    // Configure object-to-object mappings using Mapster
    // Each mapper defines type conversion rules for DTOs
    var userMapper = new UserMapper();
    userMapper.ConfigureAllMappings();

    var productMapper = new ProductMapper(builder.Configuration);
    productMapper.ConfigureAllMappings();

    var cartMapper = new Tienda_UCN_api.Src.Application.Mappers.CartMapper(builder.Configuration);
    cartMapper.ConfigureAllMappings();

    var orderMapper = new TiendaUcnApi.src.Application.Mappers.OrderMapper();
    orderMapper.ConfigureAllMappings();
    #endregion

    var app = builder.Build();

    #region Pipeline Configuration
    Log.Information("Configuring middleware pipeline");

    // Log all HTTP requests with Serilog
    app.UseSerilogRequestLogging();

    // Configure Swagger UI for API documentation (development only)
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tienda UCN API V1");
            c.RoutePrefix = string.Empty; // Serve Swagger UI at root
        });
    }

    // Global exception handling middleware
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Buyer ID middleware: manages anonymous/authenticated cart tracking
    app.UseMiddleware<TiendaUcnApi.src.API.Middleware.BuyerIdMiddleware>();

    // Hangfire Dashboard - restricted to local requests only for security
    app.UseHangfireDashboard(
        "/hangfire",
        new DashboardOptions
        {
            Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() },
        }
    );

    // Standard ASP.NET Core middleware
    app.UseHttpsRedirection();
    app.UseAuthentication(); // JWT token validation
    app.UseAuthorization(); // Role-based access control
    app.MapControllers();

    // Apply database migrations and seed initial data (roles, admin user)
    await app.SeedDatabaseAsync();

    #region Background Jobs Configuration
    // Register recurring Hangfire jobs for automated tasks

    // Job 1: Delete unconfirmed users (runs daily at 2:00 AM)
    // Removes users who haven't verified their email within the configured timeframe
    RecurringJob.AddOrUpdate<IBackgroundJobService>(
        "delete-unconfirmed-users",
        service => service.DeleteUnconfirmedUsersAsync(),
        "0 2 * * *" // Cron: Daily at 2:00 AM
    );

    // Job 2: Send abandoned cart reminders (runs daily at 12:00 PM)
    // Emails users who have items in cart but haven't purchased (inactive 3+ days)
    RecurringJob.AddOrUpdate<IBackgroundJobService>(
        "send-cart-reminders",
        service => service.SendAbandonedCartRemindersAsync(),
        "0 12 * * *" // Cron: Daily at 12:00 PM (noon)
    );
    #endregion

    #endregion

    app.Run();
}
catch (Exception ex)
{
    // Fatal error during application startup
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    // Ensure all logs are flushed before application terminates
    Log.Information("Shutting down application");
    Log.CloseAndFlush();
}
