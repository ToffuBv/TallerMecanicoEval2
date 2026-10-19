# Sistema de Gestión para Taller Mecánico - API REST

API REST desarrollada con **.NET 10** y **Entity Framework Core**, siguiendo buenas prácticas de desarrollo, principios de diseño seguro (OWASP Top 10), autenticación JWT y versionamiento de API.

---

## 🛠️ Tecnologías y Arquitectura
- **Framework:** .NET 10 (C#)
- **ORM / Base de Datos:** Entity Framework Core con Microsoft SQL Server
- **Autenticación & Autorización:** JWT (JSON Web Tokens) con roles (`Administrador`, `Mecanico`, `Recepcionista`)
- **Documentación Interactiva:** OpenAPI + Scalar API Reference (`/scalar/v1`)
- **Versionamiento:** Asp.Versioning (versión actual: `v1.0`)
- **Seguridad / OWASP:**
  - Rate Limiting global (mitigación de ataques de denegación de servicio / fuerza bruta).
  - Manejo global de excepciones para prevenir exposición de stack traces.
  - Hashing de contraseñas con PBKDF2 (`PasswordHasher<Usuario>`).
  - CORS configurado.

---

## 🚀 Requisitos Previos
1. **.NET 10 SDK** o superior.
2. **Microsoft SQL Server** (o SQL Server Express local).
3. **Visual Studio 2022/2025** o **VS Code** con la extensión de C#.

---

## ⚙️ Configuración y Puesta en Marcha

### 1. Base de Datos Automática
El proyecto cuenta con **migración automática al arrancar**.  
Al ejecutar la aplicación por primera vez en entorno de desarrollo (`Development`), Entity Framework Core creará automáticamente la base de datos `TallerMecanicoDB` y todas sus tablas si aún no existen.

### 2. Cadena de Conexión
La configuración de desarrollo se encuentra en `appsettings.Development.json`. Por defecto viene preconfigurada para SQL Server Express:

```json
"ConnectionStrings": {
  "miconexion": "Server=.\\SQLEXPRESS;Database=TallerMecanicoDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
}
```

> **Nota:** Si en tu entorno local utilizas **Autenticación de Windows** en lugar de usuario `sa`, puedes cambiar la cadena a:
> ```text
> Server=.\\SQLEXPRESS; Database=TallerMecanicoDB; Trusted_Connection=True; TrustServerCertificate=True; MultipleActiveResultSets=true
> ```

### 3. Ejecutar la Aplicación
Desde Visual Studio:
- Presiona **F5** o haz clic en el botón de inicio (perfil `https` o `http`).

Desde la terminal:
```bash
dotnet run
```

La aplicación se iniciará y abrirá automáticamente la documentación interactiva en:
👉 **`https://localhost:7027/scalar/v1`** (o `http://localhost:5290/scalar/v1`)

---

## 📦 Carga de Datos de Prueba (Seed Data)
Para poblar datos iniciales de prueba (marcas, modelos, especialidades, mecánicos y órdenes de trabajo):
- Ejecuta el script [seed.sql](./seed.sql) en SQL Server Management Studio (SSMS) o mediante Visual Studio sobre la base de datos `TallerMecanicoDB`.

---

## 🧪 Pruebas Automatizadas
Para ejecutar la suite de pruebas unitarias e integración:
```bash
dotnet test
```

---

## 👥 Roles y Acceso
- **Administrador:** Acceso completo a mantenedores y asignación.
- **Mecanico:** Gestión y actualización del estado de órdenes de trabajo.
- **Recepcionista:** Registro de clientes, vehículos e ingresos.

