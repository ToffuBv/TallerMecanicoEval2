using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Mecanico
{
    public int MecanicoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Telefono { get; set; }

    public int EspecialidadId { get; set; }

    public virtual Especialidade Especialidad { get; set; } = null!;

    public virtual ICollection<OrdenesTrabajo> OrdenesTrabajos { get; set; } = new List<OrdenesTrabajo>();
}
