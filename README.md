
# Tienda UCN - REST API

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet) ![C#](https://img.shields.io/badge/C%23-12-blue) ![SQLite](https://img.shields.io/badge/SQLite-3-blue) ![Swagger](https://img.shields.io/badge/Swagger-API-green)

**TiendaUCN API** es el componente backend de una plataforma de e-commerce moderna. Construida con ASP.NET Core 9, esta API RESTful proporciona toda la funcionalidad necesaria para gestionar usuarios, productos, carritos de compras, pedidos y autenticación, sirviendo como la columna vertebral para aplicaciones cliente (web o móvil).

Una API REST para el proyecto Tienda UCN, una plataforma de comercio electrónico. Esta API maneja la lógica de negocio, acceso a datos y seguridad para la aplicación cliente.

---

## 🚀 Inicio Rápido

```bash
# 1. Clonar el repositorio
git clone https://github.com/A-benites/TiendaUcnApi.git
cd TiendaUcnApi

# 2. Restaurar dependencias
dotnet restore

# 3. Configurar appsettings.json (ver guías abajo)
# - Cloudinary (imágenes)
# - Resend (emails)
# - JWT Secret (seguridad)

# 4. Ejecutar
dotnet run

# 5. Abrir Swagger
# https://localhost:7102/swagger
```

> **¿Primera vez?** Sigue la [Guía de Instalación Completa](INSTALL.md) para configuración paso a paso.

---

## 📖 Documentación

- **[📦 Guía de Instalación Completa](INSTALL.md)** - Instrucciones detalladas paso a paso desde cero
- **[⚡ Guía Rápida de Inicio](QUICKSTART.md)** - Pon el proyecto en marcha en 5 minutos
- **[🔧 Configuración de Servicios Externos](SERVICIOS_EXTERNOS.md)** - Guía detallada para configurar Cloudinary, Resend y JWT
- **[❓ Preguntas Frecuentes (FAQ)](FAQ.md)** - Respuestas a dudas comunes
- **[📄 Archivo de Configuración de Ejemplo](appsettings.example.json)** - Plantilla de appsettings.json
- **[📚 README Completo](#)** - Documentación completa (este archivo)

---

## 📋 Tabla de Contenidos

1. [Características Principales](#-características-principales)
2. [Arquitectura del Proyecto](#-arquitectura-del-proyecto)
3. [Tecnologías y Librerías](#-tecnologías-y-librerías)
4. [Requisitos Previos](#-requisitos-previos)
5. [Instalación Paso a Paso](#-instalación-paso-a-paso)
6. [Configuración Completa](#-configuración-completa)
7. [Ejecución del Proyecto](#-ejecución-del-proyecto)
8. [Documentación de la API](#-documentación-de-la-api)
9. [Características de Seguridad](#-características-de-seguridad)
10. [Solución de Problemas Comunes](#-solución-de-problemas-comunes)
11. [Desarrollo y Contribución](#-desarrollo-y-contribución)
12. [Licencia y Contacto](#-licencia-y-contacto)

---

## ✨ Características Principales

---

## ✨ Características Principales

### 🔐 Autenticación y Autorización
- Sistema completo de registro y login basado en tokens JWT
- Verificación de correo electrónico con códigos de verificación de tiempo limitado
- Funcionalidad de recuperación y restablecimiento de contraseñas
- Control de acceso basado en roles (Cliente y Administrador)
- Hash seguro de contraseñas con ASP.NET Core Identity
- Validación de sesiones mediante Security Stamp

### 👤 Gestión de Perfil de Usuario
- Visualización y actualización de información personal
- Funcionalidad de cambio de contraseña
- Cambio de correo electrónico con verificación
- Validación de RUT chileno
- Validación de fecha de nacimiento (edad mínima 18 años)
- Gestión de estado de usuario (activo/bloqueado)

### 📦 Gestión de Productos (Administrador)
- Operaciones CRUD completas para productos
- Carga y gestión de imágenes vía Cloudinary
- Gestión de descuentos y ofertas
- Activación/desactivación de productos (eliminación suave)
- Capacidades avanzadas de filtrado y ordenamiento
- Gestión de categorías y marcas
- Stock y control de disponibilidad

### 🛒 Carrito de Compras
- Soporte para usuarios anónimos y autenticados
- Persistencia del carrito entre sesiones
- Asociación automática del carrito cuando el usuario inicia sesión
- Cálculos de precio en tiempo real con descuentos
- Detección de carritos abandonados y recordatorios por correo electrónico

### 📋 Gestión de Pedidos
- Creación de pedidos desde el carrito de compras
- Seguimiento del estado del pedido (Pendiente, Procesando, Enviado, Entregado, Cancelado)
- Validación de transiciones de estado de pedido
- Listado paginado de pedidos con filtros
- Historial de pedidos para usuarios
- Panel de gestión de pedidos para administradores

### 📧 Notificaciones por Correo Electrónico
- Correos de bienvenida para nuevos usuarios
- Códigos de verificación de correo electrónico
- Códigos de restablecimiento de contraseña
- Recordatorios de carritos abandonados
- Soporte de correos transaccionales vía Resend
- Plantillas HTML personalizables

### ⚙️ Trabajos en Segundo Plano
- Limpieza automatizada de usuarios no verificados (Hangfire)
- Correos programados de recordatorio de carritos abandonados
- Programación de trabajos configurable
- Panel de control Hangfire para monitoreo
- Persistencia de trabajos en SQLite

---

## 🏗️ Arquitectura del Proyecto

Este proyecto sigue un enfoque de **Arquitectura Limpia (Clean Architecture)** con una clara separación de responsabilidades:

```
TiendaUcnApi/
├── src/
│   ├── API/                          # Presentation Layer
│   │   ├── Controllers/              # API endpoints
│   │   ├── Middlewares/             # Custom middleware (error handling, buyer ID)
│   │   └── Extensions/              # Service configuration and data seeding
│   │
│   ├── Application/                  # Application Layer
│   │   ├── DTO/                     # Data Transfer Objects
│   │   │   ├── AuthDTO/            # Authentication DTOs
│   │   │   ├── ProductDTO/         # Product DTOs
│   │   │   ├── CartDTO/            # Shopping cart DTOs
│   │   │   ├── OrderDTO/           # Order DTOs
│   │   │   ├── UserDTO/            # User management DTOs
│   │   │   └── BaseResponse/       # Generic response DTOs
│   │   ├── Services/
│   │   │   ├── Interfaces/         # Service contracts
│   │   │   └── Implements/         # Service implementations
│   │   ├── Mappers/                # Object mapping logic
│   │   ├── Validators/             # Custom validation attributes
│   │   ├── Exceptions/             # Custom exception types
│   │   └── Jobs/                   # Background job definitions
│   │
│   ├── Domain/                      # Domain Layer
│   │   └── Models/                 # Entity models
│   │       ├── User.cs
│   │       ├── Product.cs
│   │       ├── Category.cs
│   │       ├── Brand.cs
│   │       ├── Cart.cs
│   │       ├── Order.cs
│   │       └── ...
│   │
│   └── Infrastructure/              # Infrastructure Layer
│       ├── Data/                   # Database context and configurations
│       │   ├── AppDbContext.cs
│       │   ├── DataSeeder.cs
│       │   └── Migrations/
│       └── Repositories/
│           ├── Interfaces/         # Repository contracts
│           └── Implements/         # Repository implementations
│
├── appsettings.json                # Configuration
├── Program.cs                       # Application entry point
└── README.md                        # Project documentation
```

### Responsabilidades de las Capas

#### 1. **Capa API (Presentación)**
- **Controllers**: Manejan las peticiones y respuestas HTTP
- **Middlewares**: 
  - `ErrorHandlingMiddleware`: Manejo global de excepciones
  - `BuyerIdMiddleware`: Identificación de usuarios anónimos
- **Extensions**: Configuración de inyección de dependencias y sembrado de datos

#### 2. **Capa de Aplicación**
- **Services**: Implementación de la lógica de negocio
  - `IUserService`: Autenticación y registro de usuarios
  - `IProductService`: Gestión de productos
  - `ICartService`: Operaciones del carrito de compras
  - `IOrderService`: Procesamiento de pedidos
  - `IEmailService`: Notificaciones por correo
  - `IFileService`: Carga/eliminación de imágenes
- **DTOs**: Transferencia de datos entre capas
- **Validators**: Lógica de validación personalizada (RUT, fecha de nacimiento)
- **Mappers**: Utilidades de mapeo de objetos

#### 3. **Capa de Dominio**
- **Models**: Entidades del negocio principales
- Enumeraciones: `OrderStatus`, `Gender`, `Status`, `CodeType`
- Reglas de negocio y lógica de dominio

#### 4. **Capa de Infraestructura**
- **DbContext**: Contexto de base de datos de Entity Framework Core
- **Repositories**: Abstracción de acceso a datos
- **Migrations**: Versionado del esquema de base de datos

---

## 🛠️ Tecnologías y Librerías

### Framework Principal
- **ASP.NET Core 9**: Framework web moderno
- **C# 12**: Características más recientes del lenguaje
- **Entity Framework Core 9**: ORM para acceso a base de datos

### Base de Datos
- **SQLite**: Base de datos relacional ligera
- **Entity Framework Core**: Migraciones Code-First

### Autenticación y Seguridad
- **ASP.NET Core Identity**: Gestión de usuarios
- **JWT Bearer Tokens**: Autenticación sin estado
- **BCrypt**: Hashing de contraseñas

### Servicios Externos
- **Cloudinary**: Almacenamiento de imágenes y CDN
- **Resend**: Servicio de correo transaccional
- **Hangfire**: Programación de trabajos en segundo plano

### Herramientas de Desarrollo
- **Swagger/OpenAPI**: Documentación de API
- **Serilog**: Logging estructurado
- **Mapster**: Mapeo de objetos
- **Bogus**: Generación de datos de prueba

### Paquetes NuGet Principales
```xml
- Microsoft.AspNetCore.Authentication.JwtBearer (9.0.8)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (9.0.8)
- Microsoft.EntityFrameworkCore.Sqlite (9.0.8)
- CloudinaryDotNet (1.27.7)
- Resend (0.1.6)
- Hangfire.AspNetCore (1.8.21)
- Serilog.AspNetCore (9.0.0)
- Mapster (7.4.0)
- Bogus (35.6.3)
- Swashbuckle.AspNetCore (9.0.4)
```

---

## 📋 Requisitos Previos

> **⚡ ¿Tienes prisa?** Consulta la [Guía Rápida de Inicio](QUICKSTART.md) para poner el proyecto en marcha en 5 minutos.

> **🔧 ¿Primera vez configurando servicios externos?** Revisa la [Guía de Configuración de Servicios Externos](SERVICIOS_EXTERNOS.md) con instrucciones paso a paso detalladas.

Antes de comenzar con la instalación, asegúrate de tener instalado lo siguiente en tu sistema:
```

---

---

## � Requisitos Previos

Antes de comenzar con la instalación, asegúrate de tener instalado lo siguiente en tu sistema:

### Software Requerido

1. **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**
   - Versión mínima: 9.0
   - Verifica la instalación ejecutando: `dotnet --version`
   - Debe mostrar una versión 9.x.x

2. **[Git](https://git-scm.com/)**
   - Para clonar el repositorio
   - Verifica la instalación: `git --version`

3. **Editor de Código** (elige uno)
   - [Visual Studio 2022](https://visualstudio.microsoft.com/) (Recomendado para Windows)
   - [Visual Studio Code](https://code.visualstudio.com/) (Multiplataforma)
   - [JetBrains Rider](https://www.jetbrains.com/rider/)

4. **Herramienta de Base de Datos SQLite** (Opcional pero recomendado)
   - [DB Browser for SQLite](https://sqlitebrowser.org/)
   - [SQLite Extension para VS Code](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite)

### Cuentas de Servicios Externos Requeridas

#### 1. **Cloudinary** (Para almacenamiento de imágenes)
- **Crear cuenta gratuita**: [https://cloudinary.com/users/register/free](https://cloudinary.com/users/register/free)
- **Qué necesitas obtener**:
  - Cloud Name
  - API Key
  - API Secret
- **Dónde encontrar las credenciales**:
  1. Inicia sesión en Cloudinary
  2. Ve a Dashboard
  3. Encontrarás las credenciales en la sección "Account Details"

#### 2. **Resend** (Para envío de correos electrónicos)
- **Crear cuenta gratuita**: [https://resend.com/signup](https://resend.com/signup)
- **Qué necesitas obtener**:
  - API Key
- **Cómo obtener la API Key**:
  1. Inicia sesión en Resend
  2. Ve a "API Keys" en el menú lateral
  3. Haz clic en "Create API Key"
  4. Dale un nombre (ej: "TiendaUCN-Dev")
  5. Copia la clave generada (solo se muestra una vez)

> **⚠️ Nota Importante**: Guarda estas credenciales de forma segura. Las necesitarás durante la configuración.

### Herramientas Opcionales

- **[Postman](https://www.postman.com/)**: Para probar la API (también puedes usar Swagger)
- **[REST Client para VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)**: Para usar el archivo `.http` incluido

---

## 🚀 Instalación Paso a Paso

Sigue estas instrucciones detalladamente para instalar y configurar el proyecto desde cero.

### Paso 1: Clonar el Repositorio

Abre una terminal (PowerShell, CMD o Git Bash) y ejecuta:

```bash
git clone https://github.com/A-benites/TiendaUcnApi.git
```

### Paso 2: Navegar al Directorio del Proyecto

```bash
cd TiendaUcnApi
```

### Paso 3: Verificar la Instalación de .NET

Asegúrate de tener .NET 9 instalado:

```bash
dotnet --version
```

Deberías ver una salida como `9.0.x`. Si no es así, descarga e instala .NET 9 SDK.

### Paso 4: Restaurar Dependencias

Restaura todos los paquetes NuGet necesarios:

```bash
dotnet restore
```

Este comando descargará e instalará todas las dependencias especificadas en `TiendaUcnApi.csproj`.

**Salida esperada**:
```
Determining projects to restore...
Restored c:\...\TiendaUcnApi.csproj (in XXX ms).
```

---

## ⚙️ Configuración Completa

> **💡 Tip**: Puedes usar el archivo [`appsettings.example.json`](appsettings.example.json) como plantilla. Solo cópialo, renómbralo a `appsettings.json` y actualiza los valores.

> **📚 Guía Detallada**: Para instrucciones paso a paso sobre cómo obtener cada credencial, consulta [SERVICIOS_EXTERNOS.md](SERVICIOS_EXTERNOS.md)

### Configuración del Archivo appsettings.json

El proyecto incluye un archivo `appsettings.json` con valores de ejemplo. **DEBES actualizar** las siguientes secciones con tus propias credenciales:

#### 1. Configuración de Cloudinary

Reemplaza los valores de ejemplo con tus credenciales de Cloudinary:

```json
"Cloudinary": {
  "CloudName": "TU_CLOUD_NAME",
  "ApiKey": "TU_API_KEY",
  "ApiSecret": "TU_API_SECRET"
}
```

**Dónde encontrar estos valores**:
- Inicia sesión en [Cloudinary](https://cloudinary.com)
- Ve a Dashboard → Account Details
- Copia Cloud Name, API Key y API Secret

#### 2. Configuración de Resend (Correo Electrónico)

Reemplaza con tu API Key de Resend:

```json
"ResendAPIKey": "TU_RESEND_API_KEY"
```

**Cómo obtener la API Key**:
- Inicia sesión en [Resend](https://resend.com)
- Ve a API Keys
- Crea una nueva API Key
- Copia la clave generada

#### 3. Configuración de JWT Secret

**⚠️ MUY IMPORTANTE**: Cambia la clave secreta de JWT por una personalizada y segura:

```json
"JWTSecret": "TU_CLAVE_SECRETA_MUY_LARGA_Y_COMPLEJA_MINIMO_32_CARACTERES"
```

**Requisitos**:
- Mínimo 32 caracteres
- Usa una combinación de letras, números y símbolos
- **NUNCA** compartas esta clave
- **NUNCA** la subas a Git (usa variables de entorno en producción)

**Ejemplo de clave segura**:
```
"JWTSecret": "kJ8#mN2$pQ5&rT9*vX3@wZ7!yA4%bC6^dE1-fG0+hI8"
```

#### 4. Configuración del Usuario Administrador

Personaliza las credenciales del usuario administrador que se creará automáticamente:

```json
"User": {
  "AdminUser": {
    "Email": "admin@tudominio.com",
    "Password": "TuContraseñaSegura123!",
    "FirstName": "Nombre",
    "LastName": "Apellido",
    "Gender": "Masculino",  // Opciones: "Masculino", "Femenino", "Otro"
    "Rut": "12345678-9",    // RUT válido chileno
    "BirthDate": "01-01-1990",  // Formato: DD-MM-YYYY
    "PhoneNumber": "+56912345678"
  },
  "RandomUserPassword": "ContraseñaParaUsuariosAleatorios123!"
}
```

**Notas importantes**:
- El correo debe ser único
- La contraseña debe tener al menos 8 caracteres y 1 número
- El RUT debe ser válido (con dígito verificador correcto)
- La fecha de nacimiento debe indicar +18 años de edad
- El teléfono debe incluir código de país (+56 para Chile)

#### 5. Configuración de la Base de Datos (Opcional)

Por defecto, la base de datos se llama `tiendaucn.db`. Si quieres cambiar el nombre:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=nombre_personalizado.db"
}
```

### Archivo appsettings.json Completo de Ejemplo

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
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
    "Properties": {
      "Application": "TiendaUcnApi"
    }
  },
  "IdentityConfiguration": {
    "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
  },
  "Cloudinary": {
    "CloudName": "TU_CLOUD_NAME",
    "ApiKey": "TU_API_KEY",
    "ApiSecret": "TU_API_SECRET"
  },
  "JWTSecret": "TU_CLAVE_SECRETA_MUY_LARGA_Y_COMPLEJA_MINIMO_32_CARACTERES",
  "ResendAPIKey": "TU_RESEND_API_KEY",
  "Products": {
    "FewUnitsAvailable": 15,
    "DefaultImageUrl": "https://shop.songprinting.com/global/images/PublicShop/ProductSearch/prodgr_default_300.png",
    "DefaultPageSize": 10,
    "ImageAllowedExtensions": [".jpg", ".jpeg", ".png", ".webp"],
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
    "CronJobDeleteUnconfirmedUsers": "50 20 * * *",
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
      "Email": "admin@tudominio.com",
      "Password": "TuContraseñaSegura123!",
      "FirstName": "Nombre",
      "LastName": "Apellido",
      "Gender": "Otro",
      "Rut": "12345678-9",
      "BirthDate": "01-01-1990",
      "PhoneNumber": "+56912345678"
    },
    "RandomUserPassword": "ContraseñaParaUsuariosAleatorios123!"
  }
}
```

### Configuraciones Opcionales

#### Cambiar el Puerto de la Aplicación

Si quieres cambiar el puerto donde corre la aplicación, edita `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:TU_PUERTO;http://localhost:TU_PUERTO_HTTP"
    }
  }
}
```

Por defecto: `https://localhost:7102` y `http://localhost:5000`

#### Configuración de Trabajos en Segundo Plano

Puedes ajustar la frecuencia de los trabajos automáticos:

```json
"Jobs": {
  "CronJobDeleteUnconfirmedUsers": "0 2 * * *",  // Diario a las 2:00 AM
  "TimeZone": "Pacific SA Standard Time",  // Zona horaria de Chile
  "DaysOfDeleteUnconfirmedUsers": 7  // Días antes de eliminar usuarios sin verificar
}
```

**Formato Cron**: `minuto hora día mes día_semana`
- `0 2 * * *` = 2:00 AM todos los días
- `0 */6 * * *` = Cada 6 horas
- `30 14 * * 1` = 2:30 PM todos los lunes

---

## ▶️ Ejecución del Proyecto

### Primera Ejecución: Crear la Base de Datos

**IMPORTANTE**: En la primera ejecución, la aplicación creará automáticamente:
- La base de datos SQLite (`tiendaucn.db`)
- Todas las tablas necesarias
- Los roles (Administrador y Cliente)
- El usuario administrador configurado
- Un usuario de prueba (`cliente@test.com`)
- Datos de ejemplo (10 categorías, 20 marcas, 50 productos)

#### Opción 1: Ejecutar con dotnet CLI (Recomendado)

```bash
dotnet run
```

#### Opción 2: Ejecutar desde Visual Studio

1. Abre el archivo `TiendaUcnApi.sln` con Visual Studio 2022
2. Presiona `F5` o haz clic en el botón "Run"

#### Opción 3: Ejecutar desde Visual Studio Code

1. Abre el proyecto en VS Code
2. Presiona `F5` o ve a "Run" → "Start Debugging"

### Verificar que la Aplicación Está Corriendo

Una vez ejecutado, deberías ver en la consola algo similar a:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7102
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Acceder a la Documentación de Swagger

Abre tu navegador y navega a:

```
https://localhost:7102/swagger
```

o

```
http://localhost:5000/swagger
```

Aquí podrás ver todos los endpoints disponibles y probarlos interactivamente.

### Acceder al Panel de Hangfire

Para ver los trabajos en segundo plano:

```
https://localhost:7102/hangfire
```

**Nota**: Solo accesible desde localhost por seguridad.

### Detener la Aplicación

Presiona `Ctrl + C` en la terminal donde está corriendo la aplicación.

---

## 🧪 Probar la API

### ⚠️ Configuración Importante para Postman

**IMPORTANTE**: Para que los correos electrónicos funcionen correctamente en las pruebas de la colección Postman, debes configurar Resend:

#### Paso 1: Crear cuenta en Resend
1. Ve a [https://resend.com/signup](https://resend.com/signup)
2. Crea una cuenta gratuita
3. Verifica tu correo electrónico

#### Paso 2: Obtener API Key
1. Inicia sesión en Resend
2. Ve a **API Keys** en el menú lateral
3. Haz clic en **Create API Key**
4. Dale un nombre (ej: "TiendaUCN-Dev")
5. Copia la clave generada (**solo se muestra una vez**)

#### Paso 3: Configurar en appsettings.json
```json
{
  "ResendAPIKey": "re_TuAPIKeyAqui123456789"
}
```

#### Paso 4: Configurar correo remitente
```json
{
  "EmailConfiguration": {
    "From": "TiendaUCN <onboarding@resend.dev>"
  }
}
```

**Nota sobre el correo remitente**:
- En la cuenta gratuita de Resend, solo puedes enviar desde `onboarding@resend.dev`
- Los correos se enviarán a cualquier dirección que uses en las pruebas
- Para usar tu propio dominio, debes verificarlo en Resend (plan pago)

#### Flujos que usan correo electrónico en Postman:
- ✉️ **Registro de usuario** → Envía código de verificación
- ✉️ **Reenviar código de verificación** → Envía nuevo código
- ✉️ **Recuperar contraseña** → Envía código de recuperación
- ✉️ **Cambiar email** → Envía código de verificación al nuevo email

**Sin Resend configurado**: Estos endpoints fallarán con error 500 o no enviarán correos.

---

### Usuarios Creados Automáticamente

El sistema crea automáticamente estos usuarios:

#### Usuario Administrador
- **Email**: El configurado en `appsettings.json` → `User:AdminUser:Email`
- **Contraseña**: La configurada en `appsettings.json` → `User:AdminUser:Password`
- **Rol**: Administrador
- **Permisos**: Acceso completo a todos los endpoints

#### Usuario Cliente de Prueba
- **Email**: `cliente@test.com`
- **Contraseña**: `Cliente123!`
- **Rol**: Cliente
- **Permisos**: Acceso a endpoints públicos y de usuario

### Primer Inicio de Sesión

#### 1. Usando Swagger UI

1. Ve a `https://localhost:7102/swagger`
2. Busca el endpoint `POST /api/Auth/login`
3. Haz clic en "Try it out"
4. Ingresa las credenciales:
   ```json
   {
     "email": "admin@tudominio.com",
     "password": "TuContraseñaSegura123!"
   }
   ```
5. Haz clic en "Execute"
6. Copia el `accessToken` de la respuesta
7. Haz clic en el botón "Authorize" (candado) en la parte superior
8. Pega el token en el campo "Value": `Bearer tu_token_aquí`
9. Haz clic en "Authorize"

Ahora puedes probar todos los endpoints protegidos.

#### 2. Usando el archivo TiendaUcnApi.http

Si usas Visual Studio Code con la extensión REST Client:

1. Abre `TiendaUcnApi.http`
2. Actualiza las variables en la parte superior:
   ```http
   @baseUrl = https://localhost:7102/api
   @email = admin@tudominio.com
   @password = TuContraseñaSegura123!
   ```
3. Haz clic en "Send Request" sobre el request de login
4. Copia el `accessToken` de la respuesta
5. Pégalo en la variable `@token`

#### 3. Usando Postman

1. Importa la colección `TiendaUCN API.postman_collection.json`
2. Crea un nuevo request POST a `https://localhost:7102/api/Auth/login`
3. En el Body, selecciona "raw" y "JSON"
4. Ingresa:
   ```json
   {
     "email": "admin@tudominio.com",
     "password": "TuContraseñaSegura123!"
   }
   ```
5. Envía el request
6. Copia el `accessToken`
7. En requests posteriores, añade un header:
   - Key: `Authorization`
   - Value: `Bearer tu_token_aquí`

### Ejemplos de Uso Común

#### Crear un Nuevo Usuario

```http
POST /api/Auth/register
Content-Type: application/json

{
  "email": "nuevo@usuario.com",
  "password": "Usuario123!",
  "firstName": "Nuevo",
  "lastName": "Usuario",
  "rut": "19876543-2",
  "gender": "Masculino",
  "birthDate": "1995-05-15",
  "phoneNumber": "+56987654321"
}
```

#### Obtener Todos los Productos

```http
GET /api/products?page=1&pageSize=10
```

#### Agregar Producto al Carrito

```http
POST /api/cart/items
Authorization: Bearer tu_token_aquí
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

#### Crear un Pedido

```http
POST /api/orders
Authorization: Bearer tu_token_aquí
```

---

---

## 📚 Documentación de la API

### URL Base
```
https://localhost:7102/api
```
o
```
http://localhost:5000/api
```

### Autenticación

La mayoría de los endpoints requieren autenticación mediante token JWT Bearer. Incluye el token en el header Authorization:
```
Authorization: Bearer <tu_token_jwt>
```

### Roles
- **Cliente**: Usuarios regulares que pueden navegar productos, gestionar carrito y realizar pedidos
- **Administrador**: Acceso completo a todos los endpoints, incluyendo gestión de usuarios y productos

### Endpoints Principales

#### Autenticación (`/api/Auth`)
- `POST /login` - Inicio de sesión de usuario
- `POST /register` - Registro de nuevo usuario
- `POST /verify` - Verificación de correo electrónico
- `POST /resend-email-verification-code` - Reenviar código de verificación
- `POST /recover-password` - Solicitar restablecimiento de contraseña
- `PATCH /reset-password` - Restablecer contraseña con código

#### Perfil (`/api/user`)
- `GET /profile` - Obtener perfil de usuario
- `PUT /profile` - Actualizar perfil
- `PATCH /change-password` - Cambiar contraseña
- `POST /verify-email-change` - Verificar cambio de email

#### Productos (Público) (`/api/products`)
- `GET /` - Obtener todos los productos (con filtros y paginación)
- `GET /{id}` - Obtener detalles del producto

**Parámetros de consulta disponibles:**
- `page`: Número de página (default: 1)
- `pageSize`: Elementos por página (default: 10)
- `search`: Búsqueda por título
- `categoryId`: Filtrar por categoría
- `brandId`: Filtrar por marca
- `minPrice`: Precio mínimo
- `maxPrice`: Precio máximo
- `status`: Nuevo o Usado
- `sortBy`: Ordenar por (title, price, createdAt)
- `sortOrder`: asc o desc

#### Productos (Admin) (`/api/admin/products`)
- `GET /` - Obtener todos los productos para admin
- `GET /{id}` - Obtener detalles del producto para admin
- `POST /` - Crear producto
- `PUT /{id}` - Actualizar producto
- `DELETE /{id}` - Alternar disponibilidad del producto
- `POST /{id}/images` - Subir imágenes del producto
- `DELETE /{id}/images/{imageId}` - Eliminar imagen del producto
- `PATCH /{id}/discount` - Actualizar descuento del producto

#### Carrito (`/api/cart`)
- `GET /` - Obtener carrito del usuario
- `POST /items` - Agregar item al carrito
- `DELETE /items/{productId}` - Eliminar item del carrito
- `PUT /items/{productId}` - Actualizar cantidad del item
- `DELETE /` - Vaciar carrito

#### Pedidos (`/api/orders`)
- `POST /` - Crear pedido desde el carrito
- `GET /` - Obtener pedidos del usuario (paginados)
- `GET /{id}` - Obtener detalles del pedido

#### Pedidos (Admin) (`/api/admin/orders`)
- `GET /` - Obtener todos los pedidos (con filtros)
- `GET /{id}` - Obtener detalles del pedido
- `PATCH /{id}/status` - Actualizar estado del pedido

**Estados de pedido**: Pendiente, Procesando, Enviado, Entregado, Cancelado

#### Categorías (Admin) (`/api/admin/categories`)
- `GET /` - Obtener todas las categorías
- `GET /{id}` - Obtener categoría por ID
- `POST /` - Crear categoría
- `PUT /{id}` - Actualizar categoría
- `DELETE /{id}` - Eliminar categoría

#### Marcas (Admin) (`/api/admin/brands`)
- `GET /` - Obtener todas las marcas
- `GET /{id}` - Obtener marca por ID
- `POST /` - Crear marca
- `PUT /{id}` - Actualizar marca
- `DELETE /{id}` - Eliminar marca

#### Usuarios (Admin) (`/api/admin/users`)
- `GET /` - Obtener todos los usuarios (paginados)
- `GET /{id}` - Obtener detalles del usuario
- `PATCH /{id}/status` - Actualizar estado del usuario (activo/bloqueado)
- `PATCH /{id}/role` - Actualizar rol del usuario

Para ejemplos detallados de request/response, consulta el archivo `TiendaUcnApi.http` o explora la interfaz de Swagger UI.

---

---

---

## 🔒 Características de Seguridad

- **Hash de Contraseñas**: Utiliza ASP.NET Core Identity con BCrypt
- **Tokens JWT**: Autenticación sin estado con expiración configurable
- **Verificación de Email**: Requerida para activación de cuenta
- **Autorización Basada en Roles**: Permisos separados para clientes y administradores
- **Validación de Entrada**: Validación completa de DTOs
- **Configuración CORS**: Compartición de recursos de origen cruzado configurable
- **Manejo de Errores**: Middleware global de excepciones con mensajes de error sanitizados
- **Security Stamp**: Invalidación de sesiones cuando cambia el estado/rol del usuario
- **Rate Limiting**: Limitación de solicitudes configurable (puede añadirse)
- **Protección HTTPS**: Redirección automática a HTTPS en producción

### Buenas Prácticas de Seguridad Implementadas

1. **Contraseñas**:
   - Longitud mínima de 8 caracteres
   - Requiere al menos un dígito
   - Hash seguro con ASP.NET Core Identity

2. **Tokens JWT**:
   - Expiración automática
   - Security stamp validation
   - Invalidación de sesiones cuando el usuario es bloqueado o cambia de rol

3. **Verificación de Email**:
   - Códigos de verificación con expiración de 3 minutos
   - Limpieza automática de usuarios no verificados después de 7 días

4. **Validación de Datos**:
   - Validación de RUT chileno
   - Validación de edad mínima (18 años)
   - Validación de formatos de email y teléfono
   - Sanitización de entradas

---

## 🛠️ Solución de Problemas Comunes

### Problema: Error "No se pudo encontrar el archivo tiendaucn.db"

**Solución**: La base de datos se crea automáticamente en la primera ejecución. Asegúrate de:
1. Haber ejecutado `dotnet run` al menos una vez
2. Verificar que no haya errores en la consola
3. Revisar que la cadena de conexión en `appsettings.json` sea correcta

### Problema: Error "JWT secret key not configured"

**Solución**: 
1. Verifica que el `appsettings.json` tenga la clave `JWTSecret`
2. Asegúrate de que la clave sea suficientemente larga (mínimo 32 caracteres)
3. Reinicia la aplicación después de modificar el archivo

### Problema: Error al subir imágenes a Cloudinary

**Solución**:
1. Verifica que las credenciales de Cloudinary en `appsettings.json` sean correctas
2. Comprueba que el tamaño de la imagen no exceda 5MB
3. Asegúrate de que la extensión sea `.jpg`, `.jpeg`, `.png` o `.webp`
4. Verifica tu conexión a internet

### Problema: No se envían correos electrónicos

**Solución**:
1. Verifica que la API Key de Resend sea correcta
2. Comprueba que la API Key tenga los permisos necesarios
3. Revisa los logs en `logs/log-YYYYMMDD.json` para más detalles
4. Verifica tu cuota de envíos en Resend

### Problema: Error "The user is not confirmed" al iniciar sesión

**Solución**:
1. El usuario debe verificar su correo electrónico primero
2. Usa el endpoint `/api/Auth/verify` con el código enviado por email
3. Si no recibiste el código, usa `/api/Auth/resend-email-verification-code`
4. Para usuarios de prueba, estos ya están verificados automáticamente

### Problema: Error 401 Unauthorized en endpoints protegidos

**Solución**:
1. Asegúrate de incluir el header `Authorization: Bearer tu_token`
2. Verifica que el token no haya expirado
3. Inicia sesión nuevamente para obtener un token nuevo
4. Comprueba que el usuario tenga el rol necesario para el endpoint

### Problema: Hangfire Dashboard no es accesible

**Solución**:
1. Solo es accesible desde `localhost` por seguridad
2. Verifica la URL: `https://localhost:7102/hangfire`
3. Asegúrate de que la aplicación esté corriendo

### Problema: Error al compilar o restaurar paquetes

**Solución**:
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

### Problema: Puerto ya en uso

**Solución**:
1. Cambia el puerto en `Properties/launchSettings.json`
2. O detén la aplicación que esté usando el puerto:
   ```powershell
   # Windows PowerShell
   Get-Process -Id (Get-NetTCPConnection -LocalPort 7102).OwningProcess | Stop-Process
   ```

### Problema: La base de datos tiene datos corruptos

**Solución**:
```bash
# Eliminar la base de datos y recrearla
rm tiendaucn.db
dotnet run
```

**⚠️ Advertencia**: Esto eliminará todos los datos.

### Habilitar Logs Detallados

Si necesitas más información sobre errores, aumenta el nivel de logging en `appsettings.json`:

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

Los logs se guardan en la carpeta `logs/` con el formato `log-YYYYMMDD.json`.

---

---

## 📊 Esquema de Base de Datos

### Entidades Principales

#### User (Usuario)
- Usuario basado en Identity con roles (Cliente, Administrador)
- Validación de RUT (cédula de identidad chilena)
- Verificación de email requerida
- Validación de edad (18+)
- Campos: Email, FirstName, LastName, Rut, Gender, BirthDate, PhoneNumber

#### Product (Producto)
- Título, descripción, precio, descuento, stock
- Relaciones con categoría y marca
- Soporte para múltiples imágenes
- Eliminación suave (flag IsAvailable)
- Estado Nuevo/Usado

#### Cart (Carrito)
- Soporte para usuarios anónimos y autenticados
- BuyerId para seguimiento de sesión
- Cálculos automáticos de precios

#### Order (Pedido)
- Flujo de estados del pedido
- Items del pedido inmutables (snapshot al momento de compra)
- Relación con usuario

#### Category & Brand (Categoría y Marca)
- Entidades de búsqueda simples
- Contador de productos

### Relaciones
- User ↔ Orders (Uno a Muchos)
- User ↔ Verification Codes (Uno a Muchos)
- Product ↔ Category (Muchos a Uno)
- Product ↔ Brand (Muchos a Uno)
- Product ↔ Images (Uno a Muchos)
- Cart ↔ Cart Items (Uno a Muchos)
- Order ↔ Order Items (Uno a Muchos)

---

## 🧪 Pruebas

### Pruebas Manuales
Usa el archivo `TiendaUcnApi.http` con la extensión REST Client en VS Code o la interfaz de Swagger UI para pruebas interactivas.

### Datos de Prueba
El DataSeeder crea automáticamente:
- 1 usuario Administrador (configurado en appsettings.json)
- 1 usuario Cliente de prueba (cliente@test.com)
- 10 categorías de muestra
- 20 marcas de muestra
- 50 productos de muestra con imágenes

### Casos de Prueba Recomendados

1. **Flujo de Registro y Autenticación**:
   - Registrar nuevo usuario
   - Verificar email con código
   - Iniciar sesión
   - Acceder a perfil

2. **Flujo de Compra Completo**:
   - Navegar productos
   - Agregar productos al carrito
   - Modificar cantidades
   - Crear pedido
   - Ver historial de pedidos

3. **Flujo de Administración**:
   - Crear producto con imágenes
   - Aplicar descuento
   - Actualizar stock
   - Gestionar pedidos
   - Cambiar estados de pedido

---

## 📝 Notas de Desarrollo

### Agregar una Nueva Entidad

1. Crear el modelo en `src/Domain/Models/`
2. Agregar DbSet a `AppDbContext.cs`
3. Crear migración: `dotnet ef migrations add AddYourEntity`
4. Actualizar base de datos: `dotnet ef database update`
5. Crear DTOs en `src/Application/DTO/`
6. Crear interfaz e implementación del repositorio
7. Crear interfaz e implementación del servicio
8. Registrar servicios en `Program.cs`
9. Crear controlador en `src/API/Controllers/`

### Estilo de Código
- Usar comentarios de documentación XML para todas las clases y métodos públicos
- Seguir las convenciones de nombres de C#
- Usar async/await para todas las operaciones de I/O
- Implementar manejo de errores apropiado
- Validar todas las entradas de usuario

### Comandos Útiles de Entity Framework

```bash
# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef database update PreviousMigration

# Eliminar última migración (si no se ha aplicado)
dotnet ef migrations remove

# Ver SQL que se ejecutará
dotnet ef migrations script

# Generar script SQL de una migración específica
dotnet ef migrations script InitialMigration AddNewFeature
```

### Variables de Entorno para Producción

Para producción, **NO** uses `appsettings.json` para secretos. Usa variables de entorno:

```bash
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection = "tu_connection_string"
$env:JWTSecret = "tu_jwt_secret"
$env:ResendAPIKey = "tu_resend_key"
$env:Cloudinary__CloudName = "tu_cloud_name"
$env:Cloudinary__ApiKey = "tu_api_key"
$env:Cloudinary__ApiSecret = "tu_api_secret"
```

```bash
# Linux/Mac
export ConnectionStrings__DefaultConnection="tu_connection_string"
export JWTSecret="tu_jwt_secret"
export ResendAPIKey="tu_resend_key"
export Cloudinary__CloudName="tu_cloud_name"
export Cloudinary__ApiKey="tu_api_key"
export Cloudinary__ApiSecret="tu_api_secret"
```

---

---

## 👥 Contribuidores

-   **Amir Benites** - Desarrollador Backend - [@A-benites](https://github.com/A-benites)
-   **Álvaro Zapana** - Desarrollador Backend

## 🤝 Cómo Contribuir

Las contribuciones son bienvenidas. Para contribuir:

1. Haz fork del repositorio
2. Crea una rama de característica (`git checkout -b feature/CaracteristicaIncreible`)
3. Commit tus cambios (`git commit -m 'Agregar CaracteristicaIncreible'`)
4. Push a la rama (`git push origin feature/CaracteristicaIncreible`)
5. Abre un Pull Request

### Guías para Contribuir

- Sigue el estilo de código existente
- Añade comentarios XML a métodos públicos
- Escribe mensajes de commit descriptivos
- Prueba tus cambios antes de hacer el PR
- Actualiza la documentación si es necesario

---

## 📄 Licencia

Este proyecto fue desarrollado como parte de un proyecto académico en la Universidad Católica del Norte.

© 2024 Tienda UCN - Todos los derechos reservados.

---

## 📞 Soporte y Contacto

### ¿Tienes Preguntas?

Si tienes preguntas o problemas:

1. Revisa la sección [Solución de Problemas Comunes](#-solución-de-problemas-comunes)
2. Consulta la [documentación de la API](#-documentación-de-la-api)
3. Revisa los [issues existentes](https://github.com/A-benites/TiendaUcnApi/issues)
4. Abre un nuevo issue si no encuentras solución

### Reportar Bugs

Para reportar bugs, abre un issue incluyendo:
- Descripción del problema
- Pasos para reproducir
- Comportamiento esperado vs actual
- Screenshots (si aplica)
- Versión de .NET y sistema operativo

### Solicitar Funcionalidades

Para solicitar nuevas funcionalidades:
1. Abre un issue con la etiqueta "enhancement"
2. Describe la funcionalidad deseada
3. Explica el caso de uso
4. Proporciona ejemplos si es posible

---

## 🌟 Características Próximas

- [ ] Tests unitarios e integración
- [ ] Soporte para pagos (integración con Mercado Pago/Transbank)
- [ ] Notificaciones push
- [ ] Sistema de reseñas y calificaciones
- [ ] Wishlist (lista de deseos)
- [ ] Recomendaciones de productos
- [ ] Búsqueda avanzada con Elasticsearch
- [ ] Rate limiting por IP
- [ ] Caché con Redis
- [ ] Documentación API con ReDoc
- [ ] Containerización con Docker
- [ ] CI/CD con GitHub Actions

---

## 📚 Recursos Adicionales

### Documentación Oficial
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Cloudinary .NET SDK](https://cloudinary.com/documentation/dotnet_integration)
- [Resend API](https://resend.com/docs)
- [Hangfire](https://www.hangfire.io/)

### Tutoriales Recomendados
- [Clean Architecture en .NET](https://www.youtube.com/watch?v=dK4Yb6-LxAk)
- [JWT en ASP.NET Core](https://jasonwatmore.com/post/2022/01/07/net-6-jwt-authentication-tutorial-with-example-api)
- [Entity Framework Core Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)

---

## ✨ Agradecimientos

- Universidad Católica del Norte por el apoyo académico
- Comunidad de ASP.NET Core por la excelente documentación
- Cloudinary y Resend por sus servicios gratuitos para desarrollo

---

<div align="center">

**⭐ Si este proyecto te fue útil, considera darle una estrella en GitHub ⭐**

Desarrollado con ❤️ por estudiantes de la UCN

</div>
