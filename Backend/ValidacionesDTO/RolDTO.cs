using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record RolDTO(
        [property: Required(ErrorMessage = "El nombre del rol es obligatorio.")][property: StringLength(50)] string Nombre,
        [property: Required(ErrorMessage = "La descripción es obligatoria.")] string Descripcion);
}
