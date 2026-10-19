using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record MarcaDTO([property: Required(ErrorMessage = "El nombre de la marca es obligatorio.")][property: StringLength(100)] string Nombre);
}
