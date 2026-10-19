using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record VehiculoDTO(
        [property: Required(ErrorMessage = "La patente es obligatoria.")][property: StringLength(20)] string Patente,
        int? Anio,
        [property: Required(ErrorMessage = "El color es obligatorio.")] string Color,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe asignar un cliente válido.")] int ClienteId,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe asignar un modelo válido.")] int ModeloId);
}
