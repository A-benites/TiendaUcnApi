# ❓ Preguntas Frecuentes (FAQ)

Respuestas a las preguntas más comunes sobre TiendaUCN API.

---

## 📦 Instalación y Configuración

### ¿Necesito pagar por los servicios externos?

**No.** Todos los servicios tienen planes gratuitos generosos:
- **Cloudinary**: 25GB de almacenamiento y 25GB de ancho de banda mensual
- **Resend**: 3,000 emails por mes, 100 por día
- Perfecto para desarrollo, pruebas y proyectos pequeños

### ¿Por qué no puedo usar appsettings.json para producción?

Por **seguridad**. Nunca debes subir credenciales a Git. En producción usa:
- Variables de entorno
- Azure Key Vault
- AWS Secrets Manager
- Configuración del hosting

### ¿Puedo usar otro servicio en lugar de Cloudinary?

Sí. Deberías modificar `FileRepository.cs` para adaptarlo al servicio que uses (AWS S3, Azure Blob Storage, etc.).

### ¿Puedo usar otro servicio de email en lugar de Resend?

Sí. Modifica `EmailService.cs` para usar SendGrid, Mailgun, SMTP, etc.

---

## 🔐 Autenticación y Seguridad

### ¿Cuánto dura un token JWT?

Por defecto, los tokens expiran después de **7 días**. Puedes cambiar esto modificando `TokenService.cs`.

### ¿Qué pasa si cambio la contraseña de un usuario?

El sistema actualiza el **Security Stamp**, lo que invalida automáticamente todos los tokens JWT existentes del usuario. Esto significa que el usuario debe iniciar sesión nuevamente.

### ¿Por qué necesito verificar el email?

Por seguridad y para:
- Confirmar que el email es válido
- Permitir recuperación de contraseña
- Prevenir spam y cuentas falsas

### ¿Cuánto tiempo es válido el código de verificación?

**3 minutos**. Puedes cambiar esto en `appsettings.json` → `VerificationCode:ExpirationTimeInMinutes`.

### ¿Qué pasa si no verifico mi email?

El usuario no verificado:
- No puede iniciar sesión
- Será eliminado automáticamente después de 7 días (configurable)

---

## 👥 Usuarios

### ¿Cómo creo el primer usuario administrador?

Automáticamente. La primera vez que ejecutas la API, se crea usando las credenciales en `appsettings.json` → `User:AdminUser`.

### ¿Puedo tener múltiples administradores?

Sí. Un administrador puede cambiar el rol de cualquier usuario usando el endpoint:
```
PATCH /api/admin/users/{id}/role
```

### ¿Qué diferencia hay entre "bloqueado" e "inactivo"?

- **Bloqueado**: El usuario no puede iniciar sesión (admin lo bloqueó)
- **Inactivo**: El usuario no ha verificado su email aún

### Usuario de prueba `cliente@test.com` - ¿es necesario?

No es necesario en producción. Se crea automáticamente solo para facilitar las pruebas en desarrollo. Puedes eliminarlo de `DataSeeder.cs` si no lo necesitas.

---

## 🛒 Carrito y Pedidos

### ¿Cómo funciona el carrito para usuarios no autenticados?

Usa un **BuyerId** almacenado en una cookie que identifica al usuario anónimo. Cuando el usuario inicia sesión, su carrito anónimo se asocia automáticamente a su cuenta.

### ¿Cuánto tiempo se guardan los carritos abandonados?

Indefinidamente, pero después de **3 días** (configurable) de inactividad, el usuario recibe un email recordatorio.

### ¿Puedo cambiar un pedido después de crearlo?

No directamente. Los pedidos son **inmutables** por diseño para mantener integridad. Un administrador puede cambiar el **estado** del pedido, pero no los items o precios.

### ¿Qué estados puede tener un pedido?

1. **Pendiente**: Recién creado
2. **Procesando**: En preparación
3. **Enviado**: En camino
4. **Entregado**: Completado exitosamente
5. **Cancelado**: Cancelado por admin o usuario

---

## 📦 Productos

### ¿Cuántas imágenes puede tener un producto?

Ilimitadas (técnicamente), pero se recomienda **máximo 5-8** para mejor rendimiento.

### ¿Qué tamaño máximo tienen las imágenes?

**5 MB** por imagen. Configurable en `appsettings.json` → `Products:ImageMaxSizeInBytes`.

### ¿Qué formatos de imagen se aceptan?

- `.jpg` / `.jpeg`
- `.png`
- `.webp`

Configurable en `appsettings.json` → `Products:ImageAllowedExtensions`.

### ¿Las imágenes se optimizan automáticamente?

Sí. Cloudinary las transforma a:
- Ancho máximo: 1000px
- Calidad: auto (optimizada)
- Formato: auto (mejor según el navegador)

### ¿Qué pasa si elimino un producto que está en un pedido?

Los productos usan **eliminación suave** (`IsAvailable = false`). El producto sigue en la base de datos y en los pedidos históricos, pero no aparece en el catálogo.

---

## 🔧 Desarrollo

### ¿Puedo modificar la estructura del proyecto?

Sí, pero se recomienda mantener la **Clean Architecture** para facilitar mantenimiento y testing.

### ¿Cómo agrego una nueva migración?

```bash
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### ¿Cómo reinicio la base de datos?

```bash
# Opción 1: Eliminar y recrear
rm tiendaucn.db
dotnet run

# Opción 2: Revertir todas las migraciones
dotnet ef database update 0
dotnet ef database update
```

⚠️ **Advertencia**: Perderás todos los datos.

### ¿Cómo cambio el puerto?

Edita `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:TU_PUERTO;http://localhost:TU_PUERTO"
```

### ¿Puedo usar MySQL/PostgreSQL en lugar de SQLite?

Sí. Debes:
1. Instalar el paquete NuGet correspondiente
2. Cambiar `UseSqlite` por `UseMySQL` o `UseNpgsql` en `Program.cs`
3. Actualizar la cadena de conexión
4. Regenerar las migraciones

---

## 📧 Emails

### ¿Por qué no recibo los emails?

Verifica:
- API Key de Resend correcta
- Email de destino correcto
- Carpeta de spam
- Logs en `logs/log-YYYYMMDD.json`
- Dashboard de Resend

### ¿Puedo personalizar las plantillas de email?

Sí. Están en `src/Application/Templates/`. Son archivos HTML que puedes modificar.

### ¿Funciona Resend en localhost?

Sí, pero con limitaciones:
- Si usas `onboarding@resend.dev`, solo puedes enviar a tu email registrado en Resend
- Para enviar a cualquier email, necesitas un dominio verificado

---

## ⚙️ Trabajos en Segundo Plano (Hangfire)

### ¿Qué trabajos están configurados?

1. **Eliminar usuarios no verificados**: Diario a las 2:00 AM
2. **Recordatorios de carrito abandonado**: Diario a las 12:00 PM

### ¿Puedo cambiar la programación de los trabajos?

Sí. Edita `appsettings.json` → `Jobs:CronJobDeleteUnconfirmedUsers` usando formato Cron.

**Ejemplos**:
- `0 2 * * *` = Diario a las 2:00 AM
- `0 */6 * * *` = Cada 6 horas
- `30 14 * * 1` = Lunes a las 2:30 PM

### ¿Cómo accedo al panel de Hangfire?

`https://localhost:7102/hangfire` (solo desde localhost por seguridad)

### ¿Puedo agregar más trabajos?

Sí. Crea un método en `BackgroundJobService.cs` y regístralo en `Program.cs` con `RecurringJob.AddOrUpdate`.

---

## 📊 Base de Datos

### ¿Dónde está la base de datos?

En el mismo directorio del proyecto: `tiendaucn.db`

### ¿Puedo ver/editar la base de datos directamente?

Sí, usa:
- **DB Browser for SQLite**: https://sqlitebrowser.org/
- **VS Code SQLite Extension**

⚠️ **Cuidado**: Editar directamente puede romper la integridad de datos.

### ¿Los datos de ejemplo se crean siempre?

No. Solo si la tabla de productos está vacía. En ejecuciones posteriores, los datos persisten.

---

## 🚀 Despliegue

### ¿Cómo despliego en producción?

Opciones:
1. **Azure App Service** (recomendado para .NET)
2. **AWS Elastic Beanstalk**
3. **Docker + cualquier hosting**
4. **VPS (DigitalOcean, Linode, etc.)**

### ¿Necesito cambiar algo para producción?

Sí:
- Usar variables de entorno para secretos
- Cambiar `appsettings.json` → `AllowedHosts`
- Configurar CORS apropiadamente
- Usar base de datos robusta (PostgreSQL, SQL Server)
- Configurar HTTPS
- Implementar rate limiting
- Agregar monitoreo y alertas

### ¿Puedo usar Docker?

Sí. Necesitas crear un `Dockerfile`. Ejemplo básico:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TiendaUcnApi.dll"]
```

---

## 🐛 Errores Comunes

### "The entity type X requires a primary key to be defined"

**Solución**: Asegúrate de que todas las entidades en `Domain/Models` tengan una propiedad `Id`.

### "No database provider has been configured"

**Solución**: Verifica que la cadena de conexión esté correcta en `appsettings.json` y que `UseSqlite()` esté en `Program.cs`.

### "Unable to resolve service for type X"

**Solución**: Registra el servicio en `Program.cs` con `builder.Services.AddScoped<IX, X>()`.

### "A referential integrity constraint violation occurred"

**Solución**: Intentas eliminar una entidad que tiene relaciones. Usa eliminación suave o elimina las entidades relacionadas primero.

---

## 📞 Soporte

### ¿Dónde reporto bugs?

GitHub Issues: https://github.com/A-benites/TiendaUcnApi/issues

### ¿Cómo puedo contribuir?

1. Fork el repositorio
2. Crea una rama feature
3. Haz tus cambios
4. Abre un Pull Request

Ver detalles en [README.md](README.md#-cómo-contribuir)

### ¿Hay un canal de Discord/Slack?

Actualmente no. Usa GitHub Issues para discusiones y preguntas.

---

## 💡 Mejores Prácticas

### ¿Cómo estructuro nuevas features?

Sigue Clean Architecture:
1. Domain: Modelo/Entidad
2. Infrastructure: Repositorio
3. Application: Servicio + DTOs
4. API: Controller

### ¿Debo usar DTOs siempre?

**Sí**. Nunca expongas las entidades del dominio directamente en los controllers. Los DTOs:
- Controlan qué datos se exponen
- Permiten validación
- Facilitan versionado de API

### ¿Cómo manejo errores personalizados?

Crea excepciones custom en `Application/Exceptions/` y el `ErrorHandlingMiddleware` las manejará automáticamente.

---

<div align="center">

**¿No encuentras tu pregunta?**

[Abre un Issue](https://github.com/A-benites/TiendaUcnApi/issues) | [Ver README](README.md) | [Guía de Instalación](INSTALL.md)

</div>
