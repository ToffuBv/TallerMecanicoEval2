using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record EspecialidadDTO(
        [property: Required(ErrorMessage = "El nombre de la especialidad es obligatorio.")][property: StringLength(100)] string Nombre,
        [property: Required(ErrorMessage = "La descripción es obligatoria.")][property: StringLength(255)] string Descripcion);
}
