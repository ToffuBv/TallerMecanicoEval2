# Declaración de uso de IA

- Herramienta: Codex, mediante la aplicación conectada al entorno local.
- Modelo identificado por el entorno: GPT-6. No se dispone de un identificador más específico verificable.
- Plan/tipo de cuenta: **pendiente de que el estudiante indique el plan real**; no es información accesible al agente.
- Fecha: 19 de octubre de 2026.
- Alcance: generación del frontend React, integración con la API existente, pruebas, ajustes de seguridad y documentación. El estudiante debe revisar y poder explicar el resultado.

## Prompts reales del usuario

1. «Necesito que completes lo que falta, lo que seria lo del apartado frontend junto con react, subelo a un repositorio y realiza commits en español».
2. A la consulta por repositorio destino: `https://github.com/ToffuBv/TallerMecanicoEval2`.
3. «porque estas creando una carpeta backend? Las API's son el Backend, la carpeta taller mecanico es el backend, no le cambies el nombre ni crees una nueva, adapta el entorno en base a ese backend».

También se proporcionó `Trabajo2-LPW2.docx` como fuente de requisitos académicos. No se recibieron prompts independientes por cada archivo o etapa: la tarea original cubrió las siguientes partes, que ejecutó el agente de forma autónoma.

| Parte | Resultado generado y revisión realizada |
| --- | --- |
| Arquitectura | React + TypeScript + Vite, Router y Query. Se corrigió la decisión inicial de duplicar el backend: se eliminó la copia y se mantuvo TallerMecanico en su ubicación original por instrucción del usuario. |
| Consumo API | Wrapper fetch, Bearer, normalización de errores, servicios y contratos Zod ajustados a los endpoints existentes. |
| Seguridad | Token en memoria, control por roles, CSP, CORS, cabeceras y limitador del login. Se actualizó Vitest tras detectar vulnerabilidades y se corrigió el proveedor de base de datos de las pruebas. |
| Accesibilidad | Etiquetas, mensajes asociados, foco de ruta, resumen de errores, diálogo nativo y diseño adaptativo. Se añadió comprobación con axe; no se atribuye una revisión con NVDA que no se realizó. |
| Pruebas | Componentes con fetch simulado, flujo Playwright y ampliación de la integración .NET. Se corrigió la inyección inicial de axe porque CSP bloqueaba el script inline; la prueba lo carga mediante la API de evaluación del navegador, sin relajar CSP. |
| Documentación | README de ejecución, informe de decisiones, matriz de riesgos y guía de demostración. |

Estas decisiones describen el trabajo del agente. La aceptación, correcciones personales y comprensión del estudiante deben completarse tras su revisión; no se inventan decisiones ni prompts del estudiante. Tampoco se simula trabajo realizado durante cinco semanas.
