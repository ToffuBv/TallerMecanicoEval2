
## Miguel Cifuentes

## Configuración y Puesta en Marcha

### 1. Base de Datos Automática
El proyecto cuenta con **migración automática al arrancar**.  
Al ejecutar la aplicación por primera vez en entorno de desarrollo (`Development`), Entity Framework Core creará automáticamente la base de datos `TallerMecanicoDB` y todas sus tablas si aún no existen.

### 2. Cadena de Conexión
La configuración de desarrollo se encuentra en `appsettings.Development.json`. Por defecto viene configurada para SQL Server:

```json
"ConnectionStrings": {
  "miconexion": "Server=.\\SQLEXPRESS;Database=TallerMecanicoDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"
}
```

### 3. Ejecutar la Aplicación
Desde Visual Studio:
- Presiona **F5** o haz clic en el botón de inicio (perfil `https` o `http`).

Desde la terminal:
```bash
dotnet run
```

La aplicación se iniciará y abrirá automáticamente la documentación interactiva 


## 🧪 Pruebas Automatizadas
Para ejecutar la suite de pruebas unitarias e integración:
```bash
dotnet test
```

---

- **Administrador:** Acceso completo a mantenedores y asignación.
- **Mecanico:** Gestión y actualización del estado de órdenes de trabajo.
