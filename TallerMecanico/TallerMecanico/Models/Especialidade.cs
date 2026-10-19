using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Especialidade
{
    public int EspecialidadId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Mecanico> Mecanicos { get; set; } = new List<Mecanico>();
}
