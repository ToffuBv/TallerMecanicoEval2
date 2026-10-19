# Pruebas y guía de demostración

## Automatización

- `npm run build`: verifica TypeScript y crea la distribución.
- `npm test`: seis pruebas con Testing Library y fetch simulado: redirección privada, login/Bearer, credenciales incorrectas, rol rechazado, creación y errores asociados al formulario.
- `npm run test:e2e`: Chromium recorre login → crear → listar → detalle → editar → eliminar. Valida render de contenido con script como texto, axe en listado, ancho de 320 px sin scroll horizontal y foco inicial de confirmación. API simulada por Playwright; no depende de SQL Server.
- `dotnet test TallerMecanico/TallerMecanico/TallerMecanico.csproj`: conserva las tres pruebas originales y agrega login/JWT real con CRUD y roles, CORS/cabeceras y rate limiting. Pipeline ASP.NET real y almacenamiento EF InMemory.
- `npm audit`: auditoría de dependencias incluyendo desarrollo.

Los resultados de la última ejecución se registran en `RESULTADOS.md`. No se considera validada la instancia SQL Server por estas pruebas aisladas.

## Comprobación manual pendiente del estudiante

1. Iniciar SQL Server, API HTTPS y React según README; comprobar login con cada cuenta local.
2. Usar solo Tab, Shift+Tab, Enter y Escape: saltar contenido, navegar, filtrar, crear, editar y cancelar eliminación. Verificar foco visible y retorno al botón.
3. Activar NVDA y verificar etiquetas, anuncios de errores, resumen, estado de carga y confirmación. Registrar fecha y observaciones reales.
4. A 320 px, tablet y escritorio, y con zoom de navegador al 200%, comprobar que se accede a todos los campos y acciones sin scroll horizontal.
5. Enviar sin token: 401. Intentar eliminar con JWT de Mecanico: 403. El botón debe estar ausente para ese rol.
6. Crear observación `<script>alert(1)</script>`: se muestra como texto. Comprobar que no se abre un diálogo de JavaScript.
7. En herramientas de red comprobar HTTPS, Authorization Bearer, ausencia de token en URL y cabeceras de seguridad. Borrar sesión y comprobar que un acceso privado regresa a login; recargar también exige login.
8. Preflight con origen no permitido: no debe devolver Access-Control-Allow-Origin. La ausencia de esa cabecera es la evidencia de CORS, no necesariamente un estado HTTP 403.
9. Realizar seis intentos de login en menos de un minuto: el sexto devuelve 429. Esperar la siguiente ventana para continuar la demostración.

No se declara una certificación WCAG ni una revisión con lector de pantalla realizada por el agente.
