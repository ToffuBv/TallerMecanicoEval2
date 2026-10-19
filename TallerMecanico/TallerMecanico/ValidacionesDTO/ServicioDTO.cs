using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record ServicioDTO(
        [property: Required(ErrorMessage = "El nombre del servicio es obligatorio.")][property: StringLength(100)] string Nombre,
        [property: Required(ErrorMessage = "La descripción es obligatoria.")] string Descripcion,
        [property: Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0.")] decimal Costo);
}
