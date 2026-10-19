# Taller Mecánico · Evaluación 2

Aplicación React en español que consume la API REST existente de Taller Mecánico. Incluye autenticación JWT, rutas por rol y CRUD de órdenes de trabajo con clientes, vehículos y mecánicos relacionados.

## Estructura

```text
Frontend/                       React + TypeScript + Vite
TallerMecanico/TallerMecanico/    API original .NET 10 + SQL Server
docs/                           Informe, pruebas y uso de IA
```

La carpeta original `TallerMecanico` conserva su nombre y ubicación. El repositorio incluye sus fuentes directamente: no necesita submódulos. Los commits registran el trabajo realizado en esta sesión; no se simula un historial de cinco semanas.

## Requisitos

- Node.js 22.22 o superior y npm.
- SDK .NET 10, SQL Server o SQL Server Express.
- Certificado HTTPS de desarrollo de .NET de confianza.

## Iniciar la API

Desde PowerShell en la raíz:

```powershell
dotnet dev-certs https --trust
Copy-Item TallerMecanico/TallerMecanico/appsettings.example.json TallerMecanico/TallerMecanico/appsettings.Development.json
```

Edita **el archivo copiado** con tu conexión SQL Server, una clave JWT aleatoria de al menos 32 caracteres, issuer `TallerMecanicoApi` y audience `TallerMecanicoApiUsuarios`. No publiques ese archivo. Puedes usar autenticación de Windows en la conexión:

```text
Server=.\SQLEXPRESS;Database=TallerMecanicoDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True
```

Para generar cuentas y relaciones de desarrollo en una base vacía, define contraseñas propias de al menos 12 caracteres. No se incluyen contraseñas predeterminadas:

```powershell
$env:Seed__AdminPassword = Read-Host 'Contraseña local para admin'
$env:Seed__MecanicoPassword = Read-Host 'Contraseña local para mecanico'
dotnet run --project TallerMecanico/TallerMecanico --launch-profile https
```

La API aplica migraciones al arrancar. El seed se ejecuta solo en Development cuando se proporcionan ambas contraseñas; crea las cuentas `admin` y `mecanico` si no existen, y datos relacionados si faltan. No cambia la contraseña de cuentas existentes. Si usas una base ya poblada, puedes iniciar con tus cuentas actuales. Revisa la consola si SQL Server no está disponible.

- API: `https://localhost:7027/api/v1`
- Documentación en desarrollo: `https://localhost:7027/scalar/v1`

## Iniciar React

En una segunda terminal:

```powershell
cd Frontend
Copy-Item .env.example .env
npm ci
npm run dev
```

Abre `http://127.0.0.1:5173`. `VITE_API_URL` apunta a la API HTTPS; no contiene secretos. Los orígenes predeterminados de CORS son `http://localhost:5173` y `http://127.0.0.1:5173`. Si cambias dominio o puerto, configura `Cors:Origins` en la API y `connect-src` en `Frontend/index.html`.

## Operaciones y roles

| Operación | Admin | Mecanico |
| --- | --- | --- |
| Listar, filtrar y consultar órdenes | Sí | Sí |
| Crear y editar órdenes | Sí | Sí |
| Eliminar con confirmación | Sí | No; servidor responde 403 |

El token se conserva en memoria durante cinco minutos como máximo. Recargar exige iniciar sesión nuevamente. Cerrar sesión borra el token y la caché. La API valida firma, vencimiento y rol; leer el payload en React solo permite presentar la interfaz.

## Verificaciones

```powershell
cd Frontend
npm run build
npm test
npx playwright install chromium
npm run test:e2e
npm audit
cd ..
dotnet test TallerMecanico/TallerMecanico/TallerMecanico.csproj
```

Las pruebas de componentes y Playwright simulan respuestas HTTP. La suite .NET utiliza el pipeline real de autenticación y una base InMemory independiente de SQL Server. La prueba integrada real contra SQL Server requiere tu instancia local y las credenciales configuradas. Consulta [las evidencias](docs/PRUEBAS.md), el [informe técnico](docs/INFORME_TECNICO.md) y la [bitácora de IA](docs/USO_IA.md).
