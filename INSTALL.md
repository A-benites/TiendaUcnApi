# 📦 Guía de Instalación Completa - TiendaUCN API

Esta guía te llevará paso a paso por todo el proceso de instalación, desde cero hasta tener la API funcionando completamente.

---

## 📑 Índice

1. [Preparación](#1-preparación)
2. [Instalación de Software](#2-instalación-de-software)
3. [Configuración de Servicios Externos](#3-configuración-de-servicios-externos)
4. [Clonación y Configuración del Proyecto](#4-clonación-y-configuración-del-proyecto)
5. [Primera Ejecución](#5-primera-ejecución)
6. [Verificación](#6-verificación)
7. [Próximos Pasos](#7-próximos-pasos)

---

## 1️⃣ Preparación

### Tiempo Estimado: 5 minutos

Antes de comenzar, necesitas crear cuentas en estos servicios gratuitos:

- [ ] **Cloudinary** - Para almacenamiento de imágenes
- [ ] **Resend** - Para envío de correos electrónicos

> 💡 **Tip**: Puedes hacer esto mientras se descargan los instaladores en el siguiente paso.

---

## 2️⃣ Instalación de Software

### Tiempo Estimado: 10-15 minutos

### A. Instalar .NET 9 SDK

1. **Descargar**: Ve a https://dotnet.microsoft.com/download/dotnet/9.0
2. **Selecciona tu sistema operativo**: Windows, macOS o Linux
3. **Descarga el SDK** (no solo el Runtime)
4. **Ejecuta el instalador** y sigue las instrucciones
5. **Verifica la instalación**:
   ```bash
   dotnet --version
   ```
   Deberías ver algo como: `9.0.x`

### B. Instalar Git

1. **Descargar**: Ve a https://git-scm.com/downloads
2. **Ejecuta el instalador**
3. **Configuración recomendada**: Deja las opciones por defecto
4. **Verifica la instalación**:
   ```bash
   git --version
   ```

### C. Instalar un Editor de Código (Opcional)

#### Opción 1: Visual Studio Code (Recomendado para principiantes)
- **Descargar**: https://code.visualstudio.com/
- **Extensiones recomendadas**:
  - C# Dev Kit
  - REST Client
  - SQLite Viewer

#### Opción 2: Visual Studio 2022 (Para desarrollo .NET avanzado)
- **Descargar**: https://visualstudio.microsoft.com/
- **Workload**: Selecciona "ASP.NET and web development"

#### Opción 3: JetBrains Rider
- **Descargar**: https://www.jetbrains.com/rider/

---

## 3️⃣ Configuración de Servicios Externos

### Tiempo Estimado: 10-15 minutos

> 📚 **Guía Detallada**: Para instrucciones paso a paso con capturas de pantalla, consulta [SERVICIOS_EXTERNOS.md](SERVICIOS_EXTERNOS.md)

### A. Configurar Cloudinary

1. **Crear cuenta**: https://cloudinary.com/users/register/free
2. **Verificar email**
3. **Ir al Dashboard**
4. **Copiar credenciales**:
   - Cloud Name
   - API Key
   - API Secret
5. **Guardar** en un lugar seguro (las necesitarás después)

### B. Configurar Resend

1. **Crear cuenta**: https://resend.com/signup
2. **Ir a "API Keys"**
3. **Crear nueva API Key**:
   - Name: `TiendaUCN-Development`
   - Permission: `Full Access`
4. **Copiar la clave** (solo se muestra una vez)
5. **Guardar** en un lugar seguro

### C. Generar JWT Secret

Genera una clave segura de al menos 32 caracteres:

#### Método 1: Generador Online
```
Visita: https://generate-random.org/api-key-generator
Configura: Length 64, Alphanumeric + Special
```

#### Método 2: PowerShell (Windows)
```powershell
-join ((48..57) + (65..90) + (97..122) + (33..47) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

#### Método 3: Linux/Mac
```bash
openssl rand -base64 48
```

**Guarda esta clave** en un lugar seguro.

---

## 4️⃣ Clonación y Configuración del Proyecto

### Tiempo Estimado: 5 minutos

### A. Clonar el Repositorio

Abre una terminal y ejecuta:

```bash
# Navega a la carpeta donde quieres el proyecto
cd Documents

# Clona el repositorio
git clone https://github.com/A-benites/TiendaUcnApi.git

# Entra al directorio
cd TiendaUcnApi
```

### B. Restaurar Dependencias

```bash
dotnet restore
```

Espera a que se descarguen todos los paquetes NuGet.

### C. Configurar appsettings.json

1. **Abre el proyecto** en tu editor de código favorito

2. **Localiza el archivo** `appsettings.json` en la raíz del proyecto

3. **Actualiza las siguientes secciones**:

#### Cloudinary
```json
"Cloudinary": {
  "CloudName": "tu_cloud_name_de_cloudinary",
  "ApiKey": "tu_api_key_de_cloudinary",
  "ApiSecret": "tu_api_secret_de_cloudinary"
}
```

#### Resend
```json
"ResendAPIKey": "tu_api_key_de_resend"
```

#### JWT Secret
```json
"JWTSecret": "tu_clave_secreta_generada_minimo_32_caracteres"
```

#### Usuario Administrador
```json
"User": {
  "AdminUser": {
    "Email": "tu_email@ejemplo.com",
    "Password": "TuContraseña123!",
    "FirstName": "Tu Nombre",
    "LastName": "Tu Apellido",
    "Gender": "Masculino",  // Opciones: Masculino, Femenino, Otro
    "Rut": "12345678-9",    // RUT válido chileno
    "BirthDate": "01-01-1995",  // Formato: DD-MM-YYYY (debe ser +18 años)
    "PhoneNumber": "+56912345678"
  }
}
```

4. **Guarda el archivo**

> ⚠️ **Importante**: Asegúrate de que `appsettings.json` NO se suba a Git. Ya está incluido en `.gitignore`.

---

## 5️⃣ Primera Ejecución

### Tiempo Estimado: 2 minutos

### Ejecutar la API

En la terminal, dentro del directorio del proyecto:

```bash
dotnet run
```

### ¿Qué Sucede en la Primera Ejecución?

La aplicación automáticamente:
- ✅ Crea la base de datos SQLite (`tiendaucn.db`)
- ✅ Ejecuta todas las migraciones
- ✅ Crea los roles (Administrador y Cliente)
- ✅ Crea el usuario administrador que configuraste
- ✅ Crea un usuario de prueba (`cliente@test.com` / `Cliente123!`)
- ✅ Genera datos de ejemplo:
  - 10 categorías
  - 20 marcas
  - 50 productos

### Salida Esperada

Deberías ver algo como esto en la consola:

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

## 6️⃣ Verificación

### Tiempo Estimado: 5 minutos

### A. Verificar Swagger UI

1. **Abre tu navegador**
2. **Ve a**: `https://localhost:7102/swagger`
3. **Deberías ver**: La interfaz de Swagger con todos los endpoints

### B. Probar el Login

1. **En Swagger**, busca `POST /api/Auth/login`
2. **Click en "Try it out"**
3. **Ingresa**:
   ```json
   {
     "email": "tu_email_configurado",
     "password": "tu_password_configurado"
   }
   ```
4. **Click "Execute"**
5. **Deberías recibir**: Un `accessToken` en la respuesta

✅ **¡Perfecto!** La autenticación funciona.

### C. Autorizar en Swagger

1. **Copia el `accessToken`** de la respuesta anterior
2. **Click en el botón "Authorize"** 🔒 (arriba a la derecha)
3. **Pega**: `Bearer tu_token_aquí`
4. **Click "Authorize"**

Ahora puedes probar todos los endpoints protegidos.

### D. Verificar Cloudinary

1. **En Swagger**, busca `POST /api/admin/products`
2. **Crea un producto** con una imagen
3. **Ve a tu dashboard de Cloudinary**
4. **Verifica**: La imagen debería aparecer en Media Library

### E. Verificar Resend

1. **En Swagger**, busca `POST /api/Auth/register`
2. **Registra un nuevo usuario** con tu email
3. **Revisa tu bandeja de entrada**
4. **Deberías recibir**: Email de verificación

### F. Verificar Hangfire

1. **Abre**: `https://localhost:7102/hangfire`
2. **Deberías ver**: Panel de control de Hangfire
3. **Verifica**: Dos trabajos programados
   - `delete-unconfirmed-users`
   - `send-cart-reminders`

---

## 7️⃣ Próximos Pasos

### ¡Felicitaciones! 🎉

Tu instalación está completa. Ahora puedes:

### Explorar la API

- 📖 **Revisar la documentación**: [README.md](README.md)
- 🧪 **Probar endpoints**: Usa Swagger o el archivo `TiendaUcnApi.http`
- 📊 **Ver la base de datos**: Usa DB Browser for SQLite

### Desarrollo

- 🔨 **Crear nuevas características**: Consulta la sección "Notas de Desarrollo" en README.md
- 🧹 **Modificar el código**: El proyecto sigue Clean Architecture
- 📝 **Agregar endpoints**: Crea controllers en `src/API/Controllers/`

### Recursos

- **Documentación Completa**: [README.md](README.md)
- **Guía Rápida**: [QUICKSTART.md](QUICKSTART.md)
- **Servicios Externos**: [SERVICIOS_EXTERNOS.md](SERVICIOS_EXTERNOS.md)
- **Archivo de Ejemplo**: [appsettings.example.json](appsettings.example.json)

### Comunidad

- 🐛 **Reportar bugs**: https://github.com/A-benites/TiendaUcnApi/issues
- 💡 **Sugerir features**: Abre un issue con la etiqueta "enhancement"
- 🤝 **Contribuir**: Lee la sección "Cómo Contribuir" en README.md

---

## 🆘 ¿Problemas?

### Checklist de Verificación

- [ ] .NET 9 SDK instalado correctamente
- [ ] Git instalado
- [ ] Credenciales de Cloudinary correctas
- [ ] API Key de Resend correcta
- [ ] JWT Secret de mínimo 32 caracteres
- [ ] `dotnet restore` ejecutado sin errores
- [ ] `dotnet run` sin errores
- [ ] Base de datos `tiendaucn.db` creada

### Errores Comunes

**Error: "JWT secret key not configured"**
- Verifica que `JWTSecret` tenga mínimo 32 caracteres

**No se envían emails**
- Verifica tu API Key de Resend
- Revisa los logs en `logs/`

**Error al subir imágenes**
- Verifica las credenciales de Cloudinary
- Asegúrate de que la imagen sea menor a 5MB

**Puerto en uso**
- Cambia el puerto en `Properties/launchSettings.json`

### Más Ayuda

Consulta la sección completa de [Solución de Problemas](README.md#-solución-de-problemas-comunes) en el README.

---

<div align="center">

**✅ Instalación Completa**

¡Ya estás listo para desarrollar con TiendaUCN API!

[Volver al README](README.md) | [Guía Rápida](QUICKSTART.md) | [Reportar Problema](https://github.com/A-benites/TiendaUcnApi/issues)

</div>
