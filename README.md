
# Tienda UCN - API Rest

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet) ![C#](https://img.shields.io/badge/C%23-12-blue) ![SQLite](https://img.shields.io/badge/SQLite-3-blue) ![Swagger](https://img.shields.io/badge/Swagger-API-green)

**TiendaUCN API** es el componente backend para una moderna plataforma de e-commerce. Construida con ASP.NET Core 9, esta API Restful proporciona toda la funcionalidad necesaria para gestionar usuarios, productos, y autenticación, sirviendo como la columna vertebral para una aplicación cliente (web o móvil).

API Rest para el proyecto Tienda UCN, una plataforma de comercio electrónico. Esta API gestiona la lógica de negocio, el acceso a datos y la seguridad para la aplicación cliente.

## ✨ Features

-   **Autenticación y Autorización**: Sistema completo de registro, inicio de sesión y gestión de usuarios basado en JWT, con recuperación de contraseña y verificación de correo.
-   **Gestión de Perfiles**: Los usuarios pueden ver y actualizar la información de su perfil, así como cambiar su contraseña y verificar cambios de email.
-   **Gestión de Productos (Admin)**: Operaciones CRUD completas para productos, incluyendo carga de imágenes, gestión de descuentos y activación/desactivación de productos.
-   **Notificaciones por Email**: Envío de correos transaccionales para diversas acciones.
-   **Base de Datos**: Persistencia de datos robusta utilizando Entity Framework Core con SQLite.

## 🛠️ Tecnologías Utilizadas

-   **Framework**: ASP.NET Core 9
-   **Lenguaje**: C# 12
-   **Base de Datos**: SQLite con Entity Framework Core
-   **Autenticación**: JWT (JSON Web Tokens)
-   **Documentación API**: Swagger (OpenAPI)
-   **Logging**: Serilog
-   **Mapeo de Objetos**: Mapster
-   **Servicio de Email**: Resend

---

## 🚀 Cómo Empezar

Sigue estas instrucciones para clonar y ejecutar el proyecto en tu máquina local para desarrollo y pruebas.

### Requisitos Previos

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   [Git](https://git-scm.com/)
-   Un editor de código como [Visual Studio Code](https://code.visualstudio.com/) o [Visual Studio 2022](https://visualstudio.microsoft.com/).

### ⚙️ Instalación y Configuración

1.  **Clona el repositorio:**
    ```sh
    git clone <https://github.com/A-benites/TiendaUcnApi.git>
    ```

2.  **Navega al directorio del proyecto:**
    ```sh
    cd TiendaUcnApi
    ```

3.  **Configura tus variables de entorno:**
    Crea un archivo `appsettings.Development.json` en la raíz del proyecto con el siguiente contenido, reemplazando los placeholders con tus claves reales.

    ```json
    {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }
        },
        "ConnectionStrings": {
            "DefaultConnection": "Data Source=tienda_ucn.db"
        },
        "JWTSecret": "TU_CLAVE_SECRETA_PARA_JWT_DEBE_SER_LARGA_Y_COMPLEJA",
        "ResendAPIKey": "TU_API_KEY_DE_RESEND",
        "AdminUser": {
            "Email": "admin@example.com",
            "Password": "Admin123!",
            "FirstName": "Admin",
            "LastName": "User",
            "Rut": "1-9"
        }
    }
    ```

    > **Nota sobre el Administrador**: El sistema creará automáticamente el usuario administrador definido en la sección `AdminUser` de este archivo durante el primer arranque. Puedes modificar estos valores según necesites.

4.  **Restaura las dependencias de .NET:**
    ```sh
    dotnet restore
    ```

5.  **Aplica las migraciones de la Base de Datos:**
    El seeder creará un usuario administrador (`admin@example.com` / `Admin123!`) y datos iniciales.
    ```sh
    dotnet ef database update
    ```

6.  **Ejecuta la aplicación:**
    ```sh
    dotnet run
    ```
    La API estará corriendo en `http://localhost:5177`.

---

## 🧪 Guía Completa de Endpoints

Esta sección documenta todos los endpoints disponibles. Puedes usar **Swagger** (`http://localhost:5177`) o un cliente REST con los siguientes ejemplos.

### Variables

```http
@baseUrl = http://localhost:5177
@user_token =
@admin_token =
```
> **Instrucción**: Después de iniciar sesión, copia el token JWT en la variable `@user_token` o `@admin_token` para usar en las solicitudes protegidas.

### 1. Controlador de Autenticación (`/api/Auth`)

#### `POST /login`
Inicia sesión y devuelve un token JWT.
```http
###
POST {{baseUrl}}/api/Auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```

#### `POST /register`
Registra un nuevo usuario.
```http
###
POST {{baseUrl}}/api/Auth/register
Content-Type: application/json

{
  "email": "test.user2@example.com",
  "password": "password123",
  "firstName": "Usuario",
  "lastName": "DePrueba",
  "rut": "22333444-5",
  "birthDate": "1998-08-10"
}
```

#### `POST /verify`
Verifica el email de un usuario usando el código recibido.
```http
###
POST {{baseUrl}}/api/Auth/verify
Content-Type: application/json

{
  "email": "test.user2@example.com",
  "code": "CODIGO_DE_VERIFICACION"
}
```

#### `POST /resend-email-verification-code`
Reenvía el código de verificación de email.
```http
###
POST {{baseUrl}}/api/Auth/resend-email-verification-code
Content-Type: application/json

{
  "email": "test.user2@example.com"
}
```

#### `POST /recover-password`
Inicia el flujo de recuperación de contraseña enviando un código al email.
```http
###
POST {{baseUrl}}/api/Auth/recover-password
Content-Type: application/json

{
  "email": "test.user2@example.com"
}
```

#### `PATCH /reset-password`
Establece una nueva contraseña usando el código de recuperación.
```http
###
PATCH {{baseUrl}}/api/Auth/reset-password
Content-Type: application/json

{
  "email": "test.user2@example.com",
  "code": "CODIGO_DE_RECUPERACION",
  "newPassword": "nuevaPassword456"
}
```

### 2. Controlador de Perfil (`/api/user`)

**Nota**: Requiere token de autenticación de usuario (`@user_token`).

#### `GET /profile`
Obtiene los datos del perfil del usuario autenticado.
```http
###
GET {{baseUrl}}/api/user/profile
Authorization: Bearer {{user_token}}
```

#### `PUT /profile`
Actualiza los datos del perfil del usuario.
```http
###
PUT {{baseUrl}}/api/user/profile
Content-Type: application/json
Authorization: Bearer {{user_token}}

{
  "firstName": "NombreActualizado",
  "lastName": "ApellidoActualizado",
  "rut": "22333444-5",
  "birthDate": "1998-08-11"
}
```

#### `PATCH /change-password`
Cambia la contraseña del usuario.
```http
###
PATCH {{baseUrl}}/api/user/change-password
Content-Type: application/json
Authorization: Bearer {{user_token}}

{
  "currentPassword": "password123",
  "newPassword": "nuevaPassword456"
}
```

#### `POST /verify-email-change`
Verifica un cambio de correo electrónico.
```http
###
POST {{baseUrl}}/api/user/verify-email-change
Content-Type: application/json
Authorization: Bearer {{user_token}}

{
  "code": "CODIGO_DE_VERIFICACION_DE_EMAIL"
}
```

### 3. Controlador de Productos (Admin) (`/api/admin/products`)

**Nota**: Requiere token de autenticación de administrador (`@admin_token`).

#### `GET /`
Obtiene una lista de todos los productos con filtros opcionales.
```http
###
GET {{baseUrl}}/api/admin/products?PageNumber=1&PageSize=10
Authorization: Bearer {{admin_token}}
```

#### `GET /{id}`
Obtiene el detalle de un producto específico por su ID.
```http
###
GET {{baseUrl}}/api/admin/products/1
Authorization: Bearer {{admin_token}}
```

#### `POST /`
Crea un nuevo producto. Usa `multipart/form-data`.
```http
###
POST {{baseUrl}}/api/admin/products
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW
Authorization: Bearer {{admin_token}}

------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Name"

Producto de Prueba
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Description"

Descripción del producto.
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Price"

99990
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Stock"

50
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="CategoryId"

1
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="BrandId"

1
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="Images"; filename="image.jpg"
Content-Type: image/jpeg

< /path/to/your/image.jpg
------WebKitFormBoundary7MA4YWxkTrZu0gW--
```

#### `PUT /{id}`
Actualiza un producto existente.
```http
###
PUT {{baseUrl}}/api/admin/products/1
Content-Type: application/json
Authorization: Bearer {{admin_token}}

{
  "name": "Producto Actualizado",
  "description": "Nueva descripción.",
  "price": 109990,
  "stock": 45,
  "categoryId": 1,
  "brandId": 1
}
```

#### `DELETE /{id}`
Desactiva (eliminado lógico) un producto.
```http
###
DELETE {{baseUrl}}/api/admin/products/1
Authorization: Bearer {{admin_token}}
```

#### `POST /{id}/images`
Sube una o más imágenes para un producto.
```http
###
POST {{baseUrl}}/api/admin/products/1/images
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW
Authorization: Bearer {{admin_token}}

------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="images"; filename="image1.jpg"
Content-Type: image/jpeg

< /path/to/your/image1.jpg
------WebKitFormBoundary7MA4YWxkTrZu0gW
Content-Disposition: form-data; name="images"; filename="image2.png"
Content-Type: image/png

< /path/to/your/image2.png
------WebKitFormBoundary7MA4YWxkTrZu0gW--
```

#### `DELETE /{id}/images/{imageId}`
Elimina una imagen específica de un producto.
```http
###
DELETE {{baseUrl}}/api/admin/products/1/images/1
Authorization: Bearer {{admin_token}}
```

#### `PATCH /{id}/discount`
Actualiza el descuento de un producto.
```http
###
PATCH {{baseUrl}}/api/admin/products/1/discount
Content-Type: application/json
Authorization: Bearer {{admin_token}}

{
  "discount": 15
}
```

#### `PATCH /{id}/status`
Cambia el estado de un producto (activo/inactivo).
```http
###
PATCH {{baseUrl}}/api/admin/products/1/status
Authorization: Bearer {{admin_token}}
```

---

## 👥 Colaboradores

-   **Amir Benites**
-   **Álvaro Zapana**

