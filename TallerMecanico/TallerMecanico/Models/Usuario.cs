using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RolId { get; set; }

    public virtual Role Rol { get; set; } = null!;
}
