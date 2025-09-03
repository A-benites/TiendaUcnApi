using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaUcnApi.src.Domain.Models;
using TiendaUcnApi.src.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios para la inyección de dependencias.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, Role>()
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

app.Run();