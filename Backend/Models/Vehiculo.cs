using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Vehiculo
{
    public int VehiculoId { get; set; }

    public string Patente { get; set; } = null!;

    public int? Anio { get; set; }

    public string? Color { get; set; }

    public int ClienteId { get; set; }

    public int ModeloId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual Modelo Modelo { get; set; } = null!;

    public virtual ICollection<OrdenesTrabajo> OrdenesTrabajos { get; set; } = new List<OrdenesTrabajo>();
}
