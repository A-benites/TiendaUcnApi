# 🔧 Guía de Configuración de Servicios Externos

Esta guía te ayudará a configurar paso a paso todos los servicios externos necesarios para TiendaUCN API.

---

## 📸 Cloudinary (Almacenamiento de Imágenes)

### ¿Qué es Cloudinary?
Cloudinary es un servicio en la nube para almacenar y gestionar imágenes. Lo usamos para guardar las imágenes de los productos.

### Crear Cuenta Gratuita

1. **Visita**: https://cloudinary.com/users/register/free
2. **Completa el formulario**:
   - Email
   - Contraseña
   - Nombre de usuario
3. **Verifica tu email**: Revisa tu bandeja de entrada y haz click en el link de verificación
4. **Inicia sesión**

### Obtener Credenciales

1. **Una vez dentro**, ve al **Dashboard**
2. **Busca la sección "Account Details"** (generalmente arriba)
3. **Copia las siguientes credenciales**:

   ```
   Cloud Name: dxxxxx
   API Key: 123456789012345
   API Secret: aBcDeFgHiJkLmNoPqRsTuVwXyZ
   ```

4. **IMPORTANTE**: Haz click en el ícono del ojo 👁️ para ver el API Secret completo

### Configurar en TiendaUCN API

Abre `appsettings.json` y reemplaza:

```json
"Cloudinary": {
  "CloudName": "dxxxxx",
  "ApiKey": "123456789012345",
  "ApiSecret": "aBcDeFgHiJkLmNoPqRsTuVwXyZ"
}
```

### Configuraciones Opcionales en Cloudinary

#### Crear una carpeta para organizar imágenes

1. Ve a **Media Library**
2. Click en **Create Folder**
3. Nombra la carpeta: `tienda-ucn-productos`

Si quieres usar esta carpeta, modifica el código en `FileRepository.cs` agregando el parámetro `Folder`:

```csharp
var uploadParams = new ImageUploadParams
{
    File = new FileDescription(fileName, memoryStream),
    Folder = "tienda-ucn-productos", // Agregar esta línea
    // ...
};
```

### Límites del Plan Gratuito

- ✅ 25 GB de almacenamiento
- ✅ 25 GB de ancho de banda mensual
- ✅ Transformaciones ilimitadas
- ✅ Perfecto para desarrollo y pruebas

### Verificar que Funciona

1. Ejecuta la API
2. Inicia sesión como admin
3. Crea un producto con imagen
4. Ve a Cloudinary → Media Library
5. Deberías ver la imagen subida

---

## 📧 Resend (Servicio de Email)

### ¿Qué es Resend?
Resend es un servicio moderno para enviar emails transaccionales (verificación, recuperación de contraseña, etc.).

### Crear Cuenta Gratuita

1. **Visita**: https://resend.com/signup
2. **Regístrate con**:
   - GitHub (recomendado)
   - O con tu email
3. **Verifica tu cuenta**

### Obtener API Key

1. **Una vez dentro**, ve al menú lateral
2. **Click en "API Keys"**
3. **Click en "Create API Key"**
4. **Configura la API Key**:
   - Name: `TiendaUCN-Development`
   - Permission: `Full Access` (para desarrollo)
   - Domain: `All Domains`
5. **Click "Create"**
6. **IMPORTANTE**: Copia la clave inmediatamente (solo se muestra una vez)
   ```
   re_xxxxxxxxxxxxxxxxxxxxxxxxx
   ```

### Configurar en TiendaUCN API

Abre `appsettings.json` y reemplaza:

```json
"ResendAPIKey": "re_xxxxxxxxxxxxxxxxxxxxxxxxx"
```

### Configurar Dominio de Envío (Opcional pero Recomendado)

#### Para Desarrollo (Sin dominio propio)

Usa el dominio de prueba de Resend:

```json
"EmailConfiguration": {
  "From": "TiendaUCN <onboarding@resend.dev>",
  ...
}
```

**Limitación**: Solo puedes enviar emails a tu propia dirección registrada en Resend.

#### Para Producción (Con dominio propio)

1. **En Resend, ve a "Domains"**
2. **Click "Add Domain"**
3. **Ingresa tu dominio**: `tudominio.com`
4. **Configura los registros DNS** que Resend te proporciona:
   - MX records
   - TXT records (SPF, DKIM)
5. **Espera la verificación** (puede tomar hasta 48 horas)
6. **Actualiza el remitente**:

```json
"EmailConfiguration": {
  "From": "Tienda UCN <noreply@tudominio.com>",
  ...
}
```

### Límites del Plan Gratuito

- ✅ 3,000 emails por mes
- ✅ 100 emails por día
- ✅ 1 dominio verificado
- ✅ Perfecto para desarrollo

### Verificar que Funciona

1. Ejecuta la API
2. Registra un nuevo usuario
3. Revisa el email de verificación
4. Ve a Resend → "Emails" para ver el log de envío

### Solución de Problemas

#### "Error: API key is invalid"
- ✅ Verifica que copiaste la clave completa
- ✅ No debe tener espacios al inicio o final
- ✅ Debe empezar con `re_`

#### "No recibo emails"
- ✅ Revisa la carpeta de spam
- ✅ Verifica que el email de destino sea el mismo registrado en Resend (si usas onboarding@resend.dev)
- ✅ Revisa los logs en Resend → "Emails"
- ✅ Verifica los logs de la API en `logs/log-YYYYMMDD.json`

---

## 🔐 JWT Secret (Seguridad)

### ¿Qué es JWT Secret?

Es una clave secreta que se usa para firmar los tokens de autenticación. **DEBE ser segura y única**.

### Generar una Clave Segura

#### Opción 1: Generador Online

1. Visita: https://generate-random.org/api-key-generator
2. Configura:
   - Length: 64
   - Format: Alphanumeric + Special
3. Click "Generate"
4. Copia la clave

#### Opción 2: PowerShell (Windows)

```powershell
-join ((48..57) + (65..90) + (97..122) + (33..47) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

#### Opción 3: OpenSSL (Linux/Mac)

```bash
openssl rand -base64 48
```

#### Opción 4: Node.js

```bash
node -e "console.log(require('crypto').randomBytes(48).toString('hex'))"
```

### Requisitos de Seguridad

- ✅ Mínimo 32 caracteres (recomendado 64+)
- ✅ Incluir letras mayúsculas y minúsculas
- ✅ Incluir números
- ✅ Incluir símbolos especiales
- ❌ NO usar palabras comunes
- ❌ NO reutilizar de otros proyectos
- ❌ NO compartir o subir a Git

### Ejemplo de Clave Segura

```
kJ8#mN2$pQ5&rT9*vX3@wZ7!yA4%bC6^dE1-fG0+hI8~lK5(mP2)qR9<sU6>vY3
```

### Configurar en TiendaUCN API

```json
"JWTSecret": "TU_CLAVE_GENERADA_AQUI"
```

### Para Producción

**NUNCA** uses el `appsettings.json` en producción. Usa variables de entorno:

#### Windows PowerShell
```powershell
$env:JWTSecret = "TU_CLAVE_SEGURA"
```

#### Linux/Mac
```bash
export JWTSecret="TU_CLAVE_SEGURA"
```

#### Docker
```dockerfile
ENV JWTSecret="TU_CLAVE_SEGURA"
```

#### Azure App Service
1. Ve a Configuration
2. Application Settings
3. Add New Application Setting
   - Name: `JWTSecret`
   - Value: `tu_clave_segura`

---

## ✅ Checklist de Configuración

### Cloudinary
- [ ] Cuenta creada en cloudinary.com
- [ ] Email verificado
- [ ] Cloud Name copiado
- [ ] API Key copiado
- [ ] API Secret copiado
- [ ] Credenciales agregadas a `appsettings.json`
- [ ] Probado subiendo una imagen de producto

### Resend
- [ ] Cuenta creada en resend.com
- [ ] API Key generada
- [ ] API Key copiada y guardada
- [ ] API Key agregada a `appsettings.json`
- [ ] Probado enviando email de verificación

### JWT Secret
- [ ] Clave segura generada (mínimo 32 caracteres)
- [ ] Clave agregada a `appsettings.json`
- [ ] No compartida ni subida a Git

---

## 🆘 Ayuda Adicional

### Recursos Oficiales

- **Cloudinary**: https://cloudinary.com/documentation
- **Resend**: https://resend.com/docs
- **JWT**: https://jwt.io/introduction

### Soporte

Si tienes problemas, abre un issue en: https://github.com/A-benites/TiendaUcnApi/issues

---

<div align="center">

**¡Ya tienes todos los servicios configurados! 🎉**

Vuelve al [README.md](README.md) para continuar con la instalación.

</div>
