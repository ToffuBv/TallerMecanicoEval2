# Informe técnico

## Diseño y arquitectura

Se conserva el backend en `TallerMecanico/TallerMecanico`. React 19, TypeScript y Vite implementan el frontend en `Frontend`. React Router administra `/login`, `/ordenes`, `/ordenes/nueva`, `/ordenes/:id`, `/ordenes/:id/editar` y `/acceso-denegado`. Los componentes reutilizan encabezados, alertas, estados y confirmación. Los tipos y esquemas Zod reflejan los nombres camelCase de la API.

`api.ts` centraliza fetch, URL de entorno, cabecera Bearer y normalización de errores. `contracts.ts` valida datos entrantes y formularios. `auth.tsx` administra la sesión. `App.tsx` compone las vistas y guards. TanStack Query separa carga asíncrona del estado local de formularios y filtros, comparte catálogos e invalida las consultas después de mutaciones. Búsqueda y filtro de estado operan en el listado descargado; no se presenta como paginación del servidor.

## Autenticación y autorización

El login envía `{nombreUsuario,password}` a `/api/v1/usuarios/login`. JWT vive exclusivamente en memoria: reduce persistencia del token ante acceso posterior al navegador, pero implica perder sesión al recargar y no protege frente a un XSS activo. No se almacena en localStorage, sessionStorage, URL ni logs. Se programa cierre al llegar a `exp`, se limpia toda la caché al salir y un 401 con sesión activa provoca retorno al login. No se implementa refresh token.

Admin y Mecanico pueden operar órdenes; solo Admin elimina. La API valida roles en sus grupos y en DELETE, independientemente del guard o del botón visible. El rol se obtiene del claim estándar de .NET; la firma se valida exclusivamente en el servidor. El modelo de negocio permite a ambos roles ver las órdenes del taller: no se afirma aislamiento por mecánico ni protección multiempresa.

## Seguridad y matriz de riesgos

| Riesgo | Referencia | Lado | Mitigación y ubicación | Evidencia |
| --- | --- | --- | --- | --- |
| XSS en observaciones | Web A03 Injection | Cliente | React renderiza texto, sin dangerouslySetInnerHTML; CSP limita scripts al mismo origen en index.html | Playwright crea observación con `<script>` y la comprueba como texto |
| Robo/persistencia de JWT | API2 Broken Authentication | Cliente | Memoria, expiración, logout y limpieza de caché en auth.tsx; no URLs ni almacenamiento persistente | Prueba login y Bearer; revisión del código |
| Respuestas malformadas | API10 Unsafe Consumption | Cliente | Zod valida listas y detalle antes del render en api.ts | Contratos explícitos; error de formato controlado |
| Secretos en bundle | API8 Security Misconfiguration | Cliente | VITE_API_URL es pública; configuraciones locales ignoradas | .env.example sin claves y .gitignore |
| Dependencias vulnerables | Web A06 | Cliente | package-lock versionado; Vitest actualizado después de auditoría | npm audit sin hallazgos al verificar |
| Acceso indebido por función | API5 | Ambos | ProtectedRoute, acciones por rol y autorización del backend | Test de rol rechazado en React y DELETE con Mecanico devuelve 403 real |
| Entradas inválidas | Web A03 | Ambos | Zod y validación DTO; estados permitidos y comprobación de relaciones cliente/vehículo/mecánico en API | Formulario inválido y API devuelve 400 |
| Fuerza bruta | API4 | API y cliente | Login 5 solicitudes/minuto/IP; global 100/minuto/IP; mensaje para 429 | Integración verifica sexto intento |
| CORS y filtración interna | API8 | API | Orígenes explícitos, nosniff, DENY, no-referrer, no-store, HSTS fuera de desarrollo; errores sin detalles internos | Preflight ajeno sin Allow-Origin y pruebas de cabeceras |
| Inventario público | API9 | API | OpenAPI/Scalar solo en Development | Condición de entorno en Program.cs |

Bearer se adjunta explícitamente por JavaScript; no hay cookies de autenticación enviadas automáticamente por el navegador. Por eso este flujo no depende de tokens anti-CSRF. CORS controla lectura desde otros orígenes y no sustituye la autenticación. HTTPS protege el transporte a la API incluso cuando Vite se sirve en HTTP local. En despliegue deben servirse ambos sobre HTTPS y configurarse los orígenes reales. La CSP del frontend debe enviarse también como cabecera en el hosting, con `frame-ancestors 'none'`; esa directiva no funciona en meta.

La referencia directa a System.Security.Cryptography.Pkcs 10.0.13 corrige las advertencias heredadas de dependencias transitivas. Referencia: [aviso publicado por dotnet/runtime](https://github.com/advisories/GHSA-846f-f764-76mx).

## Accesibilidad y adaptación

Documento en español, landmarks header/main/nav/footer, enlace para saltar contenido, etiquetas visibles y errores enlazados por aria-describedby/aria-invalid. Las alertas usan role=alert; cargas y conteos role=status. Los cambios de ruta enfocan el h1 y los errores enfocan su resumen. El diálogo nativo modal contiene el foco, permite Escape y devuelve el foco al disparador al cerrarse. El rol no depende solo del color.

CSS mobile-first con Grid y Flexbox, columnas que se reorganizan, controles de al menos 46 px y foco visible. El listado cambia a tarjetas de dos columnas en móvil; el formulario queda en una columna. La prueba de navegador valida que a 320 px no exista desbordamiento horizontal y ejecuta axe en el listado. Esto no certifica por sí solo WCAG AA: resta la comprobación manual con teclado, NVDA y zoom 200% descrita en PRUEBAS.md.

## Ajustes al backend existente

Se restringió CORS a los orígenes de Vite, se agregaron cabeceras y rate limit específico para login y se eliminó el detalle interno de errores en desarrollo. Se eliminó la tolerancia adicional al vencimiento JWT, se valida el login antes de consultar y se comprueban estados y relaciones de órdenes en servidor. Los datos de desarrollo se crean solo con contraseñas proporcionadas por configuración, sin credenciales fijas versionadas. La suite original se conserva y amplía; la configuración de EF en tests elimina el registro del proveedor SQL Server antes de usar InMemory.
