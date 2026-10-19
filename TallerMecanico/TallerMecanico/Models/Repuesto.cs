using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Repuesto
{
    public int RepuestoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public virtual ICollection<DetalleRepuesto> DetalleRepuestos { get; set; } = new List<DetalleRepuesto>();
}
