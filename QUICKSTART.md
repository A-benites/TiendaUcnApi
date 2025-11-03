# 🚀 Guía Rápida de Inicio - TiendaUCN API

Esta es una guía rápida para poner en marcha el proyecto en **5 minutos**.

## ⚡ Inicio Rápido (5 Minutos)

### 1️⃣ Clonar y Restaurar (1 min)

```bash
git clone https://github.com/A-benites/TiendaUcnApi.git
cd TiendaUcnApi
dotnet restore
```

### 2️⃣ Configurar Credenciales (2 min)

Abre `appsettings.json` y actualiza SOLO estas 3 secciones:

#### A. Cloudinary (Imágenes)
```json
"Cloudinary": {
  "CloudName": "TU_CLOUD_NAME",
  "ApiKey": "TU_API_KEY",
  "ApiSecret": "TU_API_SECRET"
}
```
👉 Obtén las credenciales en: https://cloudinary.com → Dashboard

#### B. Resend (Emails)
```json
"ResendAPIKey": "TU_RESEND_API_KEY"
```
👉 Obtén la clave en: https://resend.com → API Keys

#### C. JWT Secret (Seguridad)
```json
"JWTSecret": "TuClaveSecretaMuyLargaYComplejaDeMinimoTreintaYDosCaracteres123!"
```
👉 Genera una clave segura de mínimo 32 caracteres

### 3️⃣ Ejecutar (1 min)

```bash
dotnet run
```

### 4️⃣ Probar (1 min)

Abre en tu navegador:
```
https://localhost:7102/swagger
```

---

## 🎯 Primeros Pasos

### Login con Usuario Admin

1. En Swagger, busca `POST /api/Auth/login`
2. Click en "Try it out"
3. Usa las credenciales que configuraste en `appsettings.json`:
   ```json
   {
     "email": "tu_email_de_admin",
     "password": "tu_password_de_admin"
   }
   ```
4. Copia el `accessToken` de la respuesta
5. Click en el botón "Authorize" 🔒 (arriba a la derecha)
6. Pega: `Bearer tu_token_aquí`

### Login con Usuario de Prueba

```json
{
  "email": "cliente@test.com",
  "password": "Cliente123!"
}
```

---

## 📍 URLs Importantes

- **Swagger UI**: https://localhost:7102/swagger
- **Panel Hangfire**: https://localhost:7102/hangfire
- **API Base**: https://localhost:7102/api

---

## 🔥 Endpoints Más Usados

### Autenticación
```http
POST /api/Auth/login
POST /api/Auth/register
```

### Productos
```http
GET  /api/products              # Ver todos los productos
GET  /api/products/{id}         # Ver un producto
POST /api/admin/products        # Crear producto (Admin)
```

### Carrito
```http
GET    /api/cart                # Ver mi carrito
POST   /api/cart/items          # Agregar al carrito
DELETE /api/cart/items/{id}     # Eliminar del carrito
```

### Pedidos
```http
POST /api/orders                # Crear pedido
GET  /api/orders                # Ver mis pedidos
```

---

## 💡 Tips Útiles

### Ver Logs
```bash
# Los logs se guardan en:
logs/log-YYYYMMDD.json
```

### Resetear Base de Datos
```bash
rm tiendaucn.db
dotnet run
```

### Cambiar Puerto
Edita `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:TU_PUERTO"
```

---

## 🆘 Problemas Comunes

### Error: "JWT secret key not configured"
✅ Verifica que `JWTSecret` en `appsettings.json` tenga mínimo 32 caracteres

### No se envían emails
✅ Verifica tu API Key de Resend y que esté activa

### Error al subir imágenes
✅ Verifica credenciales de Cloudinary
✅ Máximo 5MB por imagen
✅ Solo: .jpg, .jpeg, .png, .webp

### Puerto en uso
```powershell
# Windows
Get-Process -Id (Get-NetTCPConnection -LocalPort 7102).OwningProcess | Stop-Process
```

---

## 📚 Más Información

Para documentación completa, consulta [README.md](README.md)

---

## ✅ Checklist de Instalación

- [ ] .NET 9 SDK instalado
- [ ] Repositorio clonado
- [ ] `dotnet restore` ejecutado
- [ ] Credenciales de Cloudinary configuradas
- [ ] API Key de Resend configurada
- [ ] JWT Secret configurado (mínimo 32 caracteres)
- [ ] `dotnet run` ejecutado exitosamente
- [ ] Swagger accesible en https://localhost:7102/swagger
- [ ] Login funcionando con usuario admin o cliente@test.com

---

<div align="center">

**¡Listo! Ya tienes TiendaUCN API corriendo 🎉**

</div>
