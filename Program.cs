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

// Bootstrap logger
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Logging Configuration
    builder.Host.UseSerilog(
        (context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
    );
    #endregion

    #region Database Configuration
    Log.Information("Configuring SQLite database");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
    #endregion

    #region Hangfire Configuration
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

            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

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
    Log.Information("Configuring dependency injection");

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<IFileRepository, FileRepository>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<ICartRepository, CartRepository>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IUserAdminService, UserAdminService>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IBrandRepository, BrandRepository>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IBrandService, BrandService>();

    // Register background job service
    builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();
    #endregion

    #region Email Service Configuration
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
                        message = "Validation errors",
                        errors,
                        timestamp = DateTime.UtcNow,
                    }
                );
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    // Configure Mapster mappers
    var userMapper = new UserMapper();
    userMapper.ConfigureAllMappings();

    var productMapper = new ProductMapper(builder.Configuration);
    productMapper.ConfigureAllMappings();

    var cartMapper = new Tienda_UCN_api.Src.Application.Mappers.CartMapper(builder.Configuration);
    cartMapper.ConfigureAllMappings();

    var orderMapper = new TiendaUcnApi.src.Application.Mappers.OrderMapper();
    orderMapper.ConfigureAllMappings();

    var app = builder.Build();

    #region Pipeline Configuration
    Log.Information("Configuring middleware pipeline");

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tienda UCN API V1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<TiendaUcnApi.src.API.Middleware.BuyerIdMiddleware>();

    // Hangfire Dashboard (solo local)
    app.UseHangfireDashboard(
        "/hangfire",
        new DashboardOptions
        {
            Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() },
        }
    );

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Apply migrations and seed data
    await app.SeedDatabaseAsync();

    // Register recurring jobs
    RecurringJob.AddOrUpdate<IBackgroundJobService>(
        "delete-unconfirmed-users",
        service => service.DeleteUnconfirmedUsersAsync(),
        "0 2 * * *" // Daily at 2:00 AM
    );

    RecurringJob.AddOrUpdate<IBackgroundJobService>(
        "send-cart-reminders",
        service => service.SendAbandonedCartRemindersAsync(),
        "0 12 * * *" // Daily at 12:00 PM (noon)
    );

    #endregion

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.Information("Shutting down application");
    Log.CloseAndFlush();
}
