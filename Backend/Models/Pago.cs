using System;
using System.Collections.Generic;

namespace TallerMecanico.Models;

public partial class Pago
{
    public int PagoId { get; set; }

    public int OrdenId { get; set; }

    public decimal MontoTotal { get; set; }

    public string MetodoPago { get; set; } = null!;

    public DateTime? FechaPago { get; set; }

    public string Estado { get; set; } = null!;

    public virtual OrdenesTrabajo Orden { get; set; } = null!;
}
