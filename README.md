# Tienda UCN - REST API

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet) ![C#](https://img.shields.io/badge/C%23-12-blue) ![SQLite](https://img.shields.io/badge/SQLite-3-blue) ![Swagger](https://img.shields.io/badge/Swagger-API-green)

**TiendaUCN API** es una API REST completa para una plataforma de e-commerce. Construida con ASP.NET Core 9, proporciona toda la funcionalidad necesaria para gestionar usuarios, productos, carritos de compras y pedidos.

---

## 📋 Tabla de Contenidos

1. [Características](#-características-principales)
2. [Tecnologías](#️-tecnologías-utilizadas)
3. [Requisitos Previos](#-requisitos-previos)
4. [Instalación Completa](#-instalación-completa)
5. [Configuración](#️-configuración)
6. [Primera Ejecución](#️-primera-ejecución)
7. [Uso de la API](#-uso-de-la-api)
8. [Arquitectura](#️-arquitectura-del-proyecto)
9. [Solución de Problemas](#-solución-de-problemas)
10. [Preguntas Frecuentes](#-preguntas-frecuentes)

---

## ✨ Características Principales

### 🔐 Autenticación y Autorización
- Sistema completo de registro y login con JWT
- Verificación de correo electrónico
- Recuperación de contraseñas
- Roles: Cliente y Administrador

### 👤 Gestión de Usuarios
- Perfiles de usuario
- Cambio de contraseña y email
- Validación de RUT chileno
- Edad mínima 18 años

### 📦 Gestión de Productos
- CRUD completo de productos
- Carga de imágenes (Cloudinary)
- Sistema de descuentos
- Filtrado y búsqueda avanzada
- Categorías y marcas

### 🛒 Carrito de Compras
- Carrito para usuarios anónimos y autenticados
- Persistencia entre sesiones
- Asociación automática al iniciar sesión
- Cálculos en tiempo real

### 📋 Gestión de Pedidos
- Creación de pedidos
- Estados: Pendiente, Procesando, Enviado, Entregado, Cancelado
- Historial de pedidos
- Panel de administración

### 📧 Notificaciones
- Emails de bienvenida
- Códigos de verificación
- Recuperación de contraseña
- Recordatorios de carrito abandonado

### ⚙️ Trabajos Automáticos
- Limpieza de usuarios no verificados
- Recordatorios de carritos abandonados
- Panel de control Hangfire

---

## 🛠️ Tecnologías Utilizadas

- **ASP.NET Core 9** - Framework web
- **C# 12** - Lenguaje
- **Entity Framework Core 9** - ORM
- **SQLite** - Base de datos
- **JWT Bearer Tokens** - Autenticación
- **Cloudinary** - Almacenamiento de imágenes
- **Resend** - Envío de emails
- **Hangfire** - Trabajos en segundo plano
- **Swagger** - Documentación de API
- **Serilog** - Logging
- **Mapster** - Mapeo de objetos

---

## 📋 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

### Software Requerido

1. **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**
   ```powershell
   dotnet --version  # Debe mostrar 9.0.x
   ```

2. **[Git](https://git-scm.com/)**
   ```powershell
   git --version
   ```

3. **Editor de código** (elige uno):
   - [Visual Studio Code](https://code.visualstudio.com/) (Recomendado)
     - Extensiones: C# Dev Kit, REST Client, SQLite Viewer
   - [Visual Studio 2022](https://visualstudio.microsoft.com/)
   - [JetBrains Rider](https://www.jetbrains.com/rider/)

### Servicios Externos Requeridos

> ⚠️ **CRÍTICO**: Sin estos servicios, la aplicación NO funcionará.

1. **[Cloudinary](https://cloudinary.com)** - Para almacenar imágenes de productos
   - Plan gratuito: 25GB de almacenamiento
   - Los productos requieren al menos una imagen

2. **[Resend](https://resend.com)** - Para enviar emails
   - Plan gratuito: 3,000 emails/mes
   - Necesario para verificación de usuarios

---

## 🚀 Instalación Completa

Sigue estos pasos **en orden** para instalar y configurar el proyecto.

### Paso 1: Clonar el Repositorio (2 min)

```powershell
# Navega a tu carpeta de proyectos
cd C:\Users\TuUsuario\Documents

# Clona el repositorio
git clone https://github.com/A-benites/TiendaUcnApi.git

# Entra al directorio
cd TiendaUcnApi

# Restaura las dependencias
dotnet restore
```

✅ **Deberías ver**: `Restored ... (in X sec).`

---

### Paso 2: Configurar Cloudinary (5 min)

#### 1. Crear cuenta

- Ve a: **https://cloudinary.com/users/register/free**
- Completa el formulario y verifica tu email

#### 2. Obtener credenciales

- Inicia sesión en Cloudinary
- En el **Dashboard**, busca **"Account Details"**
- Copia estas 3 credenciales:

```
Cloud Name: dxxxxx
API Key: 123456789012345
API Secret: aBcDeFgHiJkLmNoPqRsTuVwXyZ
```

💡 **Tip**: Haz clic en el ícono 👁️ para ver el API Secret completo

---

### Paso 3: Configurar Resend (5 min)

#### 1. Crear cuenta

- Ve a: **https://resend.com/signup**
- Regístrate y verifica tu email

#### 2. Crear API Key

- En el menú lateral, haz clic en **"API Keys"**
- Clic en **"Create API Key"**
- Nombre: `TiendaUCN-Development`
- Clic en **"Create"**

#### 3. Copiar la clave

⚠️ **IMPORTANTE**: La clave solo se muestra **UNA VEZ**

```
re_123abc456def789ghi012jkl345mno678
```

Cópiala y guárdala en un lugar seguro.

#### 4. Nota sobre emails

- En la cuenta gratuita, solo puedes enviar desde: `onboarding@resend.dev`
- Los emails llegarán a cualquier destinatario
- Para usar tu propio dominio, necesitas verificarlo en Resend

---

### Paso 4: Generar JWT Secret (2 min)

El JWT Secret es la clave que firma los tokens de autenticación.

**Requisitos**:
- Mínimo 32 caracteres
- Mezcla de letras, números y símbolos

#### Opción 1: Generador Online

1. Ve a: **https://generate-random.org/api-key-generator**
2. Configura:
   - Length: **64**
   - Format: **Alphanumeric + Special characters**
3. Clic en **"Generate"**
4. Copia la clave

#### Opción 2: PowerShell (Windows)

```powershell
-join ((48..57) + (65..90) + (97..122) + (33,35,36,37,38,42,43,45,46,95) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

#### Opción 3: Terminal (Linux/Mac)

```bash
openssl rand -base64 48
```

**Ejemplo de clave segura**:
```
kJ8#mN2$pQ5&rT9*vX3@wZ7!yA4%bC6^dE1-fG0+hI8.nL5_oP2@qR9#sT3$uV7
```

---

## ⚙️ Configuración

### Paso 5: Configurar appsettings.json (10 min)

> 💡 **Archivo de Referencia**: El proyecto incluye `appsettings.example.json` como plantilla. Puedes usarlo como referencia.

1. **Abre el proyecto** en tu editor de código:
   ```powershell
   code .  # Si usas VS Code
   ```

2. **Abre el archivo** `appsettings.json` (en la raíz del proyecto)

3. **Reemplaza COMPLETAMENTE** el contenido con la siguiente configuración:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "DefaultConnection": "Data Source=tiendaucn.db"
    },
    "Serilog": {
        "MinimumLevel": {
            "Default": "Information",
            "Override": {
                "Microsoft.AspNetCore": "Warning",
                "System": "Warning"
            }
        },
        "WriteTo": [
            {
                "Name": "Console",
                "Args": {
                    "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
                }
            },
            {
                "Name": "File",
                "Args": {
                    "path": "logs/log-.json",
                    "rollingInterval": "Day",
                    "retainedFileCountLimit": 14,
                    "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog"
                }
            }
        ],
        "Enrich": [
            "FromLogContext",
            "WithMachineName",
            "WithThreadId"
        ],
        "Properties": {
            "Application": "TiendaUcnApi"
        }
    },
    "IdentityConfiguration": {
        "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
    },
    "Cloudinary": {
        "CloudName": "TU_CLOUD_NAME_AQUI",
        "ApiKey": "TU_API_KEY_AQUI",
        "ApiSecret": "TU_API_SECRET_AQUI"
    },
    "JWTSecret": "CAMBIA_ESTA_CLAVE_POR_UNA_MUY_LARGA_Y_SEGURA_DE_MINIMO_32_CARACTERES",
    "ResendAPIKey": "TU_RESEND_API_KEY_AQUI",
    "Products": {
        "FewUnitsAvailable": 15,
        "DefaultImageUrl": "https://shop.songprinting.com/global/images/PublicShop/ProductSearch/prodgr_default_300.png",
        "DefaultPageSize": 10,
        "ImageAllowedExtensions": [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ],
        "ImageMaxSizeInBytes": 5242880,
        "TransformationWidth": 1000,
        "TransformationCrop": "scale",
        "TransformationQuality": "auto:best",
        "TransformationFetchFormat": "auto"
    },
    "CookieExpirationDays": 30,
    "Cart": {
        "AbandonedCartDays": 3
    },
    "Jobs": {
        "CronJobDeleteUnconfirmedUsers": "0 2 * * *",
        "TimeZone": "Pacific SA Standard Time",
        "DaysOfDeleteUnconfirmedUsers": 7
    },
    "HangfireDashboard": {
        "StatsPollingInterval": 5000,
        "DashboardTitle": "Panel de Control de Hangfire",
        "DashboardPath": "/hangfire",
        "DisplayStorageConnectionString": false
    },
    "VerificationCode": {
        "ExpirationTimeInMinutes": 3
    },
    "EmailConfiguration": {
        "From": "TiendaUCN <onboarding@resend.dev>",
        "WelcomeSubject": "Bienvenido a la Tienda UCN",
        "VerificationSubject": "Código de verificación",
        "PasswordResetSubject": "Restablece tu contraseña de Tienda UCN"
    },
    "User": {
        "AdminUser": {
            "Email": "admin@ejemplo.com",
            "Password": "CambiaEstaContraseña123!",
            "FirstName": "Administrador",
            "LastName": "Sistema",
            "Gender": "Otro",
            "Rut": "12345678-9",
            "BirthDate": "01-01-1990",
            "PhoneNumber": "+56912345678"
        },
        "RandomUserPassword": "ContraseñaParaUsuariosGenerados123!"
    }
}
```

4. **Ahora PERSONALIZA** las siguientes secciones con tus datos reales:

#### A. Cloudinary (OBLIGATORIO)

Reemplaza estos valores con tus credenciales de Cloudinary:

```json
"Cloudinary": {
  "CloudName": "mi-cloud-name-real",           // ← Tu Cloud Name de Cloudinary
  "ApiKey": "123456789012345",                 // ← Tu API Key de Cloudinary
  "ApiSecret": "aBcDeFgHiJkLmNoPqRsTuVwXyZ"   // ← Tu API Secret de Cloudinary
}
```

#### B. Resend (OBLIGATORIO)

Reemplaza con tu API Key de Resend:

```json
"ResendAPIKey": "re_123abc456def789ghi012jkl345mno678"  // ← Tu API Key de Resend
```

#### C. JWT Secret (OBLIGATORIO)

Reemplaza con la clave segura que generaste (mínimo 32 caracteres):

```json
"JWTSecret": "kJ8#mN2$pQ5&rT9*vX3@wZ7!yA4%bC6^dE1-fG0+hI8.nL5_oP2@qR9#sT3$uV7"
```

⚠️ **IMPORTANTE**: Esta clave debe ser:
- Mínimo 32 caracteres
- Mezcla de letras, números y símbolos especiales
- Única para tu aplicación
- **NUNCA compartida ni subida a Git**

#### D. Usuario Administrador (RECOMENDADO: Dejar por defecto)

El proyecto viene pre-configurado con un usuario administrador que funciona con la colección de Postman:

```json
"User": {
  "AdminUser": {
    "Email": "admin@tiendaucn.cl",             // ← Email del administrador
    "Password": "Admin123!",                    // ← Contraseña del administrador
    "FirstName": "Administrador",               // ← Nombre
    "LastName": "Sistema",                      // ← Apellido
    "Gender": "Otro",                           // ← Género
    "Rut": "12345678-9",                        // ← RUT válido chileno
    "BirthDate": "01-01-1990",                  // ← Formato DD-MM-YYYY
    "PhoneNumber": "+56912345678"               // ← Con código de país
  },
  "RandomUserPassword": "ContraseñaParaUsuariosGenerados123!"
}
```

> 💡 **IMPORTANTE**: Estas credenciales están sincronizadas con la colección de Postman incluida en el proyecto. Si las cambias, también deberás actualizar las variables en Postman.

**Si decides personalizar el usuario administrador**:
- ✅ **Email**: Válido y único
- ✅ **Password**: Mínimo 8 caracteres, al menos 1 número
- ✅ **RUT**: Válido chileno (con dígito verificador correcto)
- ✅ **BirthDate**: Formato DD-MM-YYYY, mínimo 18 años
- ✅ **PhoneNumber**: Con código de país (+56 para Chile)
- ✅ **Gender**: Solo "Masculino", "Femenino", o "Otro"
- ⚠️ **Recuerda actualizar las variables en Postman**: `testAdminEmail` y `testAdminPassword`

> 📝 **Cómo actualizar variables en Postman**:
> 1. Abre Postman e importa la colección `TiendaUCN API.postman_collection.json`
> 2. Clic derecho en la colección → **Edit**
> 3. Ve a la pestaña **Variables**
> 4. Actualiza `testAdminEmail` y `testAdminPassword` con tus nuevas credenciales
> 5. Clic en **Save**

#### E. Configuraciones Opcionales (Puedes dejarlas por defecto)

Estas configuraciones ya están optimizadas, pero puedes modificarlas si lo necesitas:

**Imagen por defecto de productos**:
```json
"Products": {
  "DefaultImageUrl": "https://shop.songprinting.com/global/images/PublicShop/ProductSearch/prodgr_default_300.png"
}
```

**Base de datos**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=tiendaucn.db"  // Nombre de la BD SQLite
}
```

**Trabajos automáticos**:
```json
"Jobs": {
  "CronJobDeleteUnconfirmedUsers": "0 2 * * *",  // Diario a las 2:00 AM
  "DaysOfDeleteUnconfirmedUsers": 7               // Eliminar usuarios no verificados después de 7 días
}
```

**Carrito abandonado**:
```json
"Cart": {
  "AbandonedCartDays": 3  // Enviar recordatorio después de 3 días
}
```

**Expiración de código de verificación**:
```json
"VerificationCode": {
  "ExpirationTimeInMinutes": 3  // Los códigos expiran en 3 minutos
}
```

5. **Guarda el archivo** (Ctrl+S o Cmd+S)

### ✅ Checklist de Configuración

Antes de continuar, verifica que configuraste correctamente:

**Servicios Externos Obligatorios**:
- [ ] **Cloudinary CloudName** (sin comillas extra, sin espacios)
- [ ] **Cloudinary ApiKey** (solo números)
- [ ] **Cloudinary ApiSecret** (letras y números)
- [ ] **ResendAPIKey** (comienza con `re_`)
- [ ] **JWTSecret** (mínimo 32 caracteres, con símbolos)

**Usuario Administrador**:
- [ ] **Email** válido (formato: ejemplo@dominio.com) - Por defecto: admin@tiendaucn.cl
- [ ] **Password** válida (mínimo 8 caracteres, 1 número) - Por defecto: Admin123!
- [ ] **RUT** válido chileno (formato: 12345678-9)
- [ ] **BirthDate** correcta (formato DD-MM-YYYY, +18 años)
- [ ] **PhoneNumber** con código de país (+56912345678)
- [ ] **Gender** correcto (Masculino/Femenino/Otro)

**Archivo**:
- [ ] **appsettings.json guardado** (Ctrl+S)

> 💡 **Tip**: Si tienes dudas, compara tu `appsettings.json` con el archivo de ejemplo `appsettings.example.json` incluido en el proyecto.

> ⚠️ **IMPORTANTE**: El archivo `appsettings.json` está en `.gitignore` y **NO se subirá** a Git. Esto es correcto para proteger tus credenciales. Solo `appsettings.example.json` está en el repositorio como referencia.

---

## ▶️ Primera Ejecución

### Paso 6: Ejecutar la Aplicación (2 min)

```powershell
dotnet run
```

### ¿Qué sucede en la primera ejecución?

La aplicación automáticamente:

1. ✅ Crea la base de datos SQLite (`tiendaucn.db`)
2. ✅ Ejecuta todas las migraciones
3. ✅ Crea los roles: "Administrador" y "Cliente"
4. ✅ Crea tu usuario administrador
5. ✅ Crea un usuario de prueba:
   - Email: `cliente@test.com`
   - Password: `Cliente123!`
6. ✅ Genera datos de ejemplo:
   - 10 categorías
   - 20 marcas
   - 50 productos (sin imágenes)

### Salida Esperada

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7102
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

✅ **¡Éxito!** La API está corriendo.

---

### Paso 7: Verificar la Instalación (5 min)

#### A. Abrir Swagger

1. Abre tu navegador
2. Ve a: **https://localhost:7102/swagger**
3. Deberías ver la interfaz de Swagger con todos los endpoints

#### B. Probar el Login

1. En Swagger, busca **`POST /api/Auth/login`**
2. Clic en **"Try it out"**
3. Ingresa las credenciales del administrador:
   ```json
   {
     "email": "admin@tiendaucn.cl",
     "password": "Admin123!"
   }
   ```
4. Clic en **"Execute"**
5. ✅ **Deberías recibir**: Un `accessToken` en la respuesta

#### C. Autorizar en Swagger

1. **Copia** el `accessToken` (sin las comillas)
2. Clic en el botón **"Authorize"** 🔒 (arriba a la derecha)
3. Pega: `Bearer tu_token_aqui`
4. Clic en **"Authorize"**
5. Clic en **"Close"**

Ahora puedes probar todos los endpoints protegidos.

#### D. Acceder al Panel de Hangfire

1. Ve a: **https://localhost:7102/hangfire**
2. Deberías ver el panel de control de Hangfire
3. Verifica que hay 2 trabajos programados:
   - `delete-unconfirmed-users`
   - `send-cart-reminders`

#### E. 📮 Importar Colección de Postman (Opcional pero Recomendado)

Para probar la API de manera completa, importa la colección incluida:

1. Abre **Postman**
2. Clic en **"Import"**
3. Selecciona: `TiendaUCN API.postman_collection.json`
4. Verifica que las variables de colección coincidan con tus credenciales:
   - `testAdminEmail`: `admin@tiendaucn.cl`
   - `testAdminPassword`: `Admin123!`

> 💡 **Tip**: La colección incluye flujos completos de prueba (autenticación, compras, administración) con tests automatizados. Ver sección [Probar con Postman](#probar-con-postman) para más detalles.

---

## 📚 Uso de la API

### URLs Base

- **HTTPS**: `https://localhost:7102/api`
- **HTTP**: `http://localhost:5000/api`
- **Swagger**: `https://localhost:7102/swagger`
- **Hangfire**: `https://localhost:7102/hangfire`

### Autenticación

La mayoría de los endpoints requieren autenticación JWT. Incluye el token en el header:

```
Authorization: Bearer <tu_token>
```

### Usuarios de Prueba

#### Usuario Administrador
- Email: `admin@tiendaucn.cl`
- Password: `Admin123!`
- Rol: Administrador
- Permisos: Acceso completo
- ℹ️ Estas credenciales están sincronizadas con la colección de Postman

#### Usuario Cliente
- Email: `cliente@test.com`
- Password: `Cliente123!`
- Rol: Cliente
- Permisos: Endpoints públicos y de usuario

### Endpoints Principales

#### 🔐 Autenticación (`/api/Auth`)

```http
POST /api/Auth/login                           # Iniciar sesión
POST /api/Auth/register                        # Registrar nuevo usuario
POST /api/Auth/verify                          # Verificar email
POST /api/Auth/resend-email-verification-code  # Reenviar código
POST /api/Auth/recover-password                # Solicitar código de recuperación
PATCH /api/Auth/reset-password                 # Restablecer contraseña
```

#### 👤 Perfil (`/api/user`)

```http
GET  /api/user/profile          # Ver perfil
PUT  /api/user/profile          # Actualizar perfil
PATCH /api/user/change-password # Cambiar contraseña
POST /api/user/verify-email-change # Verificar cambio de email
```

#### 📦 Productos Públicos (`/api/products`)

```http
GET /api/products     # Listar productos (con filtros)
GET /api/products/{id} # Ver detalle del producto
```

**Parámetros de consulta**:
- `page` - Número de página (default: 1)
- `pageSize` - Items por página (default: 10)
- `search` - Buscar por título
- `categoryId` - Filtrar por categoría
- `brandId` - Filtrar por marca
- `minPrice` / `maxPrice` - Rango de precio
- `status` - Nuevo o Usado
- `sortBy` - Ordenar por (title, price, createdAt)
- `sortOrder` - asc o desc

#### 📦 Productos Admin (`/api/admin/products`)

```http
GET    /api/admin/products           # Listar todos los productos
GET    /api/admin/products/{id}      # Ver producto
POST   /api/admin/products           # Crear producto
PUT    /api/admin/products/{id}      # Actualizar producto
DELETE /api/admin/products/{id}      # Desactivar producto
POST   /api/admin/products/{id}/images # Subir imágenes
DELETE /api/admin/products/{id}/images/{imageId} # Eliminar imagen
PATCH  /api/admin/products/{id}/discount # Actualizar descuento
```

#### 🛒 Carrito (`/api/cart`)

```http
GET    /api/cart                    # Ver carrito
POST   /api/cart/items              # Agregar producto
PUT    /api/cart/items/{productId}  # Actualizar cantidad
DELETE /api/cart/items/{productId}  # Eliminar producto
DELETE /api/cart                    # Vaciar carrito
```

#### 📋 Pedidos (`/api/orders`)

```http
POST /api/orders     # Crear pedido desde el carrito
GET  /api/orders     # Listar mis pedidos
GET  /api/orders/{id} # Ver detalle del pedido
```

#### 📋 Pedidos Admin (`/api/admin/orders`)

```http
GET   /api/admin/orders           # Listar todos los pedidos
GET   /api/admin/orders/{id}      # Ver pedido
PATCH /api/admin/orders/{id}/status # Cambiar estado
```

**Estados de pedido**:
1. Pendiente
2. Procesando
3. Enviado
4. Entregado
5. Cancelado

#### 🏷️ Categorías Admin (`/api/admin/categories`)

```http
GET    /api/admin/categories     # Listar categorías
GET    /api/admin/categories/{id} # Ver categoría
POST   /api/admin/categories     # Crear categoría
PUT    /api/admin/categories/{id} # Actualizar categoría
DELETE /api/admin/categories/{id} # Eliminar categoría
```

#### 🏷️ Marcas Admin (`/api/admin/brands`)

```http
GET    /api/admin/brands     # Listar marcas
GET    /api/admin/brands/{id} # Ver marca
POST   /api/admin/brands     # Crear marca
PUT    /api/admin/brands/{id} # Actualizar marca
DELETE /api/admin/brands/{id} # Eliminar marca
```

#### 👥 Usuarios Admin (`/api/admin/users`)

```http
GET   /api/admin/users           # Listar usuarios
GET   /api/admin/users/{id}      # Ver usuario
PATCH /api/admin/users/{id}/status # Bloquear/Desbloquear
PATCH /api/admin/users/{id}/role   # Cambiar rol
```

### Ejemplos de Uso

#### 1. Registrar un nuevo usuario

```http
POST /api/Auth/register
Content-Type: application/json

{
  "email": "nuevo@usuario.com",
  "password": "Usuario123!",
  "firstName": "Juan",
  "lastName": "Pérez",
  "rut": "19876543-2",
  "gender": "Masculino",
  "birthDate": "1995-05-15",
  "phoneNumber": "+56987654321"
}
```

#### 2. Crear un producto (Admin)

```http
POST /api/admin/products
Authorization: Bearer {token}
Content-Type: multipart/form-data

title: Notebook HP Pavilion
description: Laptop de alta gama
price: 599990
stock: 10
status: Nuevo
categoryId: 1
brandId: 1
images: [archivo1.jpg, archivo2.jpg]
```

⚠️ **IMPORTANTE**: Los productos requieren **al menos una imagen**.

#### 3. Agregar al carrito

```http
POST /api/cart/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

#### 4. Crear un pedido

```http
POST /api/orders
Authorization: Bearer {token}
```

Este endpoint crea un pedido con todos los productos del carrito actual.

### Probar con Postman

El proyecto incluye una colección de Postman: `TiendaUCN API.postman_collection.json`

#### Importar la Colección

1. Abre Postman
2. Clic en **"Import"** (esquina superior izquierda)
3. Selecciona el archivo `TiendaUCN API.postman_collection.json`
4. Clic en **"Import"**

#### ⚠️ IMPORTANTE: Verificar Variables de Colección

La colección incluye variables pre-configuradas que deben coincidir con tu `appsettings.json`:

**Para verificar/editar las variables**:
1. En Postman, selecciona la colección "TiendaUCN API"
2. Clic en los **tres puntos (...)** → **"Edit"**
3. Ve a la pestaña **"Variables"**

**Variables principales** (ya configuradas por defecto):

| Variable | Valor por Defecto | Descripción |
|----------|------------------|-------------|
| `baseUrl` | `http://localhost:5000/api` | URL base de la API |
| `testAdminEmail` | `admin@tiendaucn.cl` | Email del administrador |
| `testAdminPassword` | `Admin123!` | Contraseña del administrador |
| `verifiedUserEmail` | `cliente@test.com` | Usuario cliente de prueba |
| `verifiedUserPassword` | `Cliente123!` | Contraseña del cliente |
| `testUserEmail` | `postmanflowuser@test.com` | Email para pruebas de registro |
| `testUserPassword` | `Postman123!` | Contraseña para pruebas |

> 🔑 **Credenciales Sincronizadas**: Las variables `testAdminEmail` y `testAdminPassword` están sincronizadas con el usuario administrador de `appsettings.json`. Si cambias las credenciales del admin en la configuración, **debes actualizar estas variables en Postman**.

#### Uso de los Flujos

Los requests están organizados en carpetas por funcionalidad:

1. **🔐 1. FLUJO: Autenticación Completa** - Registro, verificación y login
2. **🛒 2. FLUJO: Compra Completa (Cliente)** - Desde productos hasta crear orden
3. **🔧 3. FLUJO: Administración de Productos** - CRUD completo de productos
4. **📁 Auth** - Endpoints de autenticación individuales
5. **📁 Profile** - Gestión de perfil de usuario
6. **📁 Products** - Endpoints públicos de productos
7. **📁 Cart** - Gestión de carrito
8. **📁 Orders** - Gestión de pedidos
9. **📁 Admin** - Endpoints administrativos

#### Variables Dinámicas (Capturadas Automáticamente)

Durante la ejecución de los flujos, estas variables se capturan automáticamente:

- `authToken` - Token JWT del usuario autenticado
- `adminToken` - Token JWT del administrador
- `currentUserId` - ID del usuario actual
- `productId` - ID del producto creado/consultado
- `categoryId` - ID de categoría
- `brandId` - ID de marca

**No necesitas configurarlas manualmente**, se actualizan automáticamente con los scripts de prueba.

### Probar con archivo .http

El proyecto incluye `TiendaUcnApi.http` para usar con la extensión REST Client de VS Code:

1. Abre el archivo en VS Code
2. Actualiza las variables en la parte superior
3. Haz clic en "Send Request" sobre cada request

---

## 🏗️ Arquitectura del Proyecto

El proyecto sigue **Clean Architecture** con separación de capas:

```
TiendaUcnApi/
├── src/
│   ├── API/                    # Capa de Presentación
│   │   ├── Controllers/        # Endpoints de la API
│   │   ├── Middlewares/        # Error handling, BuyerId
│   │   └── Extensions/         # Configuración de servicios
│   │
│   ├── Application/            # Capa de Aplicación
│   │   ├── DTO/               # Data Transfer Objects
│   │   ├── Services/          # Lógica de negocio
│   │   ├── Mappers/           # Mapeo de objetos
│   │   ├── Validators/        # Validaciones personalizadas
│   │   └── Jobs/              # Trabajos en segundo plano
│   │
│   ├── Domain/                # Capa de Dominio
│   │   └── Models/            # Entidades del negocio
│   │
│   └── Infrastructure/        # Capa de Infraestructura
│       ├── Data/              # DbContext, Migrations
│       └── Repositories/      # Acceso a datos
│
├── appsettings.json           # Configuración
├── Program.cs                 # Entry point
└── tiendaucn.db              # Base de datos SQLite (se crea automáticamente)
```

### Capas

1. **API (Presentación)**
   - Controllers: Manejan HTTP requests/responses
   - Middlewares: Error handling global, identificación de usuarios anónimos
   - Extensions: Inyección de dependencias, seeding de datos

2. **Application (Aplicación)**
   - Services: Lógica de negocio
   - DTOs: Transferencia de datos entre capas
   - Validators: Validación de RUT, edad, etc.
   - Mappers: Conversión entre entidades y DTOs

3. **Domain (Dominio)**
   - Models: Entidades del negocio (User, Product, Order, etc.)
   - Enums: Gender, Status, OrderStatus, CodeType

4. **Infrastructure (Infraestructura)**
   - DbContext: Entity Framework Core
   - Repositories: Abstracción de acceso a datos
   - Migrations: Control de versiones de la BD

---

## 🐛 Solución de Problemas

### Error: "JWT secret key not configured"

**Causa**: El `JWTSecret` no está configurado o es muy corto.

**Solución**:
1. Abre `appsettings.json`
2. Verifica que `JWTSecret` tenga mínimo 32 caracteres
3. Reinicia la aplicación

---

### Error: No se envían emails

**Causa**: Resend no está configurado correctamente.

**Solución**:
1. Verifica que `ResendAPIKey` sea correcto (comienza con `re_`)
2. Revisa los logs en `logs/log-YYYYMMDD.json`
3. Verifica tu cuota en el dashboard de Resend

---

### Error al subir imágenes a Cloudinary

**Causa**: Credenciales incorrectas o problemas de conexión.

**Solución**:
1. Verifica las credenciales de Cloudinary en `appsettings.json`
2. Asegúrate de que la imagen sea menor a 5MB
3. Formatos permitidos: `.jpg`, `.jpeg`, `.png`, `.webp`
4. Revisa los logs para más detalles

---

### Error: "The user is not confirmed"

**Causa**: El usuario no ha verificado su email.

**Solución**:
1. Usa el endpoint `/api/Auth/verify` con el código recibido
2. Si no recibiste el código: `/api/Auth/resend-email-verification-code`
3. Los usuarios de prueba ya están verificados

---

### Error: 401 Unauthorized

**Causa**: Token inválido o expirado.

**Solución**:
1. Verifica que incluyes el header: `Authorization: Bearer {token}`
2. El token expira después de 7 días
3. Inicia sesión nuevamente para obtener un token nuevo
4. Verifica que el usuario tenga el rol necesario

---

### Puerto ya en uso

**Causa**: Otra aplicación usa el puerto 7102 o 5000.

**Solución**:

**Opción 1**: Detener la aplicación que usa el puerto
```powershell
# Windows PowerShell
Get-Process -Id (Get-NetTCPConnection -LocalPort 7102).OwningProcess | Stop-Process
```

**Opción 2**: Cambiar el puerto en `Properties/launchSettings.json`
```json
"applicationUrl": "https://localhost:TU_PUERTO;http://localhost:TU_PUERTO_HTTP"
```

---

### Base de datos corrupta

**Solución**: Eliminar y recrear la base de datos

⚠️ **Advertencia**: Esto eliminará todos los datos.

```powershell
# Detén la aplicación (Ctrl+C)

# Elimina la base de datos
Remove-Item tiendaucn.db

# Ejecuta la aplicación de nuevo
dotnet run
```

La base de datos se recreará automáticamente con datos de ejemplo.

---

### Error al compilar o restaurar paquetes

**Solución**:
```powershell
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

---

### Habilitar logs detallados

Si necesitas más información sobre errores, edita `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

Los logs se guardan en `logs/log-YYYYMMDD.json`.

---

## ❓ Preguntas Frecuentes

### ¿Los productos de prueba tienen imágenes?

**No**. Los 50 productos creados automáticamente **NO tienen imágenes**.

- Solo se crean con datos básicos (título, precio, stock, etc.)
- Para crear productos con imágenes, usa el endpoint `/api/admin/products`
- **Los productos requieren al menos una imagen** al crearlos

### ¿Cuál es la imagen por defecto?

La URL configurada en `appsettings.json` → `Products:DefaultImageUrl`:

```
https://shop.songprinting.com/global/images/PublicShop/ProductSearch/prodgr_default_300.png
```

Esta imagen se muestra cuando se consultan productos sin imágenes en la base de datos.

### ¿Puedo usar MySQL o PostgreSQL?

Sí. Debes:
1. Instalar el paquete NuGet correspondiente
2. Cambiar `UseSqlite` por `UseMySql` o `UseNpgsql` en `Program.cs`
3. Actualizar la cadena de conexión
4. Regenerar las migraciones

### ¿Cuánto dura un token JWT?

**7 días** por defecto. Puedes cambiarlo en `TokenService.cs`.

### ¿Qué pasa si cambio la contraseña de un usuario?

El sistema actualiza el **Security Stamp**, invalidando todos los tokens existentes. El usuario debe iniciar sesión nuevamente.

### ¿Puedo tener múltiples administradores?

Sí. Un administrador puede cambiar el rol de cualquier usuario usando:
```
PATCH /api/admin/users/{id}/role
```

### ¿Cómo funciona el carrito para usuarios anónimos?

Usa un **BuyerId** almacenado en una cookie. Cuando el usuario inicia sesión, su carrito anónimo se asocia automáticamente a su cuenta.

### ¿Cuánto tiempo se guardan los carritos?

Indefinidamente, pero después de 3 días de inactividad, el usuario recibe un email recordatorio.

### ¿Puedo cambiar un pedido después de crearlo?

No. Los pedidos son inmutables por diseño. Un administrador puede cambiar el **estado** pero no los items o precios.

### ¿Dónde está la base de datos?

En el mismo directorio del proyecto: `tiendaucn.db`

Puedes abrirla con:
- **DB Browser for SQLite**
- **VS Code SQLite Extension**

### ¿Los datos de ejemplo se crean siempre?

No. Solo si la tabla de productos está vacía. En ejecuciones posteriores, los datos persisten.

### ¿Necesito pagar por Cloudinary o Resend?

**No**. Los planes gratuitos son suficientes para desarrollo:
- **Cloudinary**: 25GB almacenamiento, 25GB bandwidth/mes
- **Resend**: 3,000 emails/mes, 100/día

### ¿Puedo usar otro servicio de email?

Sí. Modifica `EmailService.cs` para usar SendGrid, Mailgun, SMTP, etc.

### ¿Cómo accedo al panel de Hangfire?

Ve a: `https://localhost:7102/hangfire` (solo accesible desde localhost)

### ¿Qué trabajos están programados?

1. **Eliminar usuarios no verificados**: Diario a las 2:00 AM
2. **Recordatorios de carrito**: Diario a las 12:00 PM

Puedes cambiar la programación en `appsettings.json` → `Jobs:CronJobDeleteUnconfirmedUsers`

### ¿Cómo agrego una nueva entidad?

1. Crear modelo en `src/Domain/Models/`
2. Agregar DbSet a `AppDbContext.cs`
3. Crear migración: `dotnet ef migrations add AddNuevaEntidad`
4. Actualizar BD: `dotnet ef database update`
5. Crear DTOs, Repositorio, Servicio y Controller

---

## 🤝 Contribución

Las contribuciones son bienvenidas. Para contribuir:

1. Fork del repositorio
2. Crea una rama: `git checkout -b feature/CaracteristicaIncreible`
3. Commit: `git commit -m 'Agregar CaracteristicaIncreible'`
4. Push: `git push origin feature/CaracteristicaIncreible`
5. Abre un Pull Request

### Guías

- Sigue el estilo de código existente
- Añade comentarios XML a métodos públicos
- Escribe mensajes de commit descriptivos
- Prueba tus cambios
- Actualiza la documentación si es necesario

---

## 📄 Licencia

Este proyecto fue desarrollado como parte de un proyecto académico en la Universidad Católica del Norte.

© 2024 Tienda UCN - Todos los derechos reservados.

---

## 👥 Desarrolladores

- **Amir Benites** - [@A-benites](https://github.com/A-benites)
- **Álvaro Zapana**

---

## 📞 Soporte

### ¿Tienes preguntas o problemas?

1. Revisa esta documentación
2. Consulta los [issues existentes](https://github.com/A-benites/TiendaUcnApi/issues)
3. Abre un nuevo issue si no encuentras solución

### Reportar Bugs

Incluye:
- Descripción del problema
- Pasos para reproducir
- Comportamiento esperado vs actual
- Screenshots (si aplica)
- Versión de .NET y SO

---

<div align="center">

**⭐ Si este proyecto te fue útil, dale una estrella en GitHub ⭐**

Desarrollado con ❤️ por estudiantes de la UCN

</div>
