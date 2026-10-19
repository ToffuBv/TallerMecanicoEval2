using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record RepuestoDTO(
        [property: Required(ErrorMessage = "El nombre del repuesto es obligatorio.")][property: StringLength(100)] string Nombre,
        [property: Required(ErrorMessage = "La descripción es obligatoria.")] string Descripcion,
        [property: Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")] decimal Precio,
        [property: Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")] int Stock);
}
