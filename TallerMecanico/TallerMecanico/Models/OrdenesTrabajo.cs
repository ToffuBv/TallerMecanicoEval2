using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class OrdenesTrabajo
{
    public int OrdenId { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observaciones { get; set; }

    public int ClienteId { get; set; }

    public int VehiculoId { get; set; }

    public int MecanicoId { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetalleRepuesto> DetalleRepuestos { get; set; } = new List<DetalleRepuesto>();

    public virtual ICollection<DetalleServicio> DetalleServicios { get; set; } = new List<DetalleServicio>();

    public virtual Mecanico Mecanico { get; set; } = null!;

    public virtual Pago? Pago { get; set; }

    public virtual Vehiculo Vehiculo { get; set; } = null!;
}
