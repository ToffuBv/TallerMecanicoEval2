using System;
using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record OrdenTrabajoDTO(
        DateTime? FechaIngreso,
        [property: Required(ErrorMessage = "El estado de la orden es obligatorio.")] string Estado,
        string Observaciones,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente válido.")] int ClienteId,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un vehículo válido.")] int VehiculoId,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un mecánico válido.")] int MecanicoId);
}
