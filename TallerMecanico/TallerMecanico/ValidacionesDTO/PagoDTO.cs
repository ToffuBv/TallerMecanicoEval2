using System;
using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record PagoDTO(
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe asignar un ID de orden válido.")] int OrdenId,
        [property: Range(0.01, double.MaxValue, ErrorMessage = "El monto del pago debe ser mayor a 0.")] decimal MontoTotal,
        [property: Required(ErrorMessage = "El método de pago es obligatorio.")] string MetodoPago,
        DateTime? FechaPago,
        string Estado);
}
