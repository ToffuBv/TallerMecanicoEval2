using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class DetalleRepuesto
{
    public int DetalleRepuestoId { get; set; }

    public int OrdenId { get; set; }

    public int RepuestoId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual OrdenesTrabajo Orden { get; set; } = null!;

    public virtual Repuesto Repuesto { get; set; } = null!;
}
