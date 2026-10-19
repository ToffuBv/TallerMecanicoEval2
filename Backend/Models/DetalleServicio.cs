using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class DetalleServicio
{
    public int DetalleServicioId { get; set; }

    public int OrdenId { get; set; }

    public int ServicioId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual OrdenesTrabajo Orden { get; set; } = null!;

    public virtual Servicio Servicio { get; set; } = null!;
}
