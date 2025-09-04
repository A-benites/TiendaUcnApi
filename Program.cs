using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models; // Asegúrate que este namespace sea el correcto
using TiendaUcnApi.src.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios para la inyección de dependencias.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder
    .Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP. El orden es importante.
if (app.Environment.IsDevelopment())
{
    // Habilita el middleware para generar la especificación de OpenAPI y la UI de Swagger.
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirige las peticiones HTTP a HTTPS.
app.UseHttpsRedirection();

// Habilita la autenticación.
app.UseAuthentication();

// Habilita la autorización. Debe declararse después de UseAuthentication.
app.UseAuthorization();

// Mapea los endpoints a los métodos de acción de los controladores.
app.MapControllers();

// Aplica migraciones y puebla la base de datos al iniciar.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        // Llama al método del seeder.
        await DataSeeder.SeedAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error durante el poblado de la base de datos.");
    }
}


app.Run();