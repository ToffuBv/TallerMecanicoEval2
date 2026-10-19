using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Modelo
{
    public int ModeloId { get; set; }

    public string Nombre { get; set; } = null!;

    public int MarcaId { get; set; }

    public virtual Marca Marca { get; set; } = null!;

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
