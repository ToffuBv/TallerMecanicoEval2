using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record MecanicoDTO(
        [property: Required(ErrorMessage = "El nombre del mecánico es obligatorio.")][property: StringLength(100)] string Nombre,
        [property: Required(ErrorMessage = "El apellido es obligatorio.")][property: StringLength(100)] string Apellido,
        [property: Required(ErrorMessage = "El teléfono es obligatorio.")][property: StringLength(20)] string Telefono,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe asignar una especialidad válida.")] int EspecialidadId);
}
