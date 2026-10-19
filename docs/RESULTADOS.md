# Resultados de verificación

Ejecuciones realizadas el 19 de octubre de 2026 en Windows:

| Verificación | Resultado |
| --- | --- |
| Compilación React / TypeScript | Correcta |
| Vitest + Testing Library | 6 pruebas correctas |
| Playwright / Chromium | 1 flujo correcto: login, crear, listar, detalle, editar y eliminar |
| axe en listado | Sin violaciones de las etiquetas WCAG A/AA seleccionadas |
| Viewport móvil 320 × 740 | Sin desbordamiento horizontal del documento en listado |
| Suite .NET 10 | 6 pruebas correctas, incluidas las 3 originales |
| npm audit | 0 vulnerabilidades tras actualizar Vitest |
| Dependencia Pkcs | 10.0.13; restauración sin advertencias de vulnerabilidad |

Persisten advertencias AV0029/AV0030 de recomendaciones de configuración de OpenAPI por versión; no impiden compilar ni aprobar los tests de contrato. La API utilizada por Playwright se simula. Los tests .NET utilizan JWT real e InMemory, no una instancia SQL Server. Revisión manual con NVDA, teclado, zoom y prueba de la base local: pendientes del estudiante.
