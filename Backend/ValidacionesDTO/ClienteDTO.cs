using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record ClienteDTO(
        [property: Required(ErrorMessage = "El nombre es obligatorio.")][property: StringLength(100)] string Nombre,
        [property: Required(ErrorMessage = "El apellido es obligatorio.")][property: StringLength(100)] string Apellido,
        [property: Required(ErrorMessage = "El correo es obligatorio.")][property: EmailAddress(ErrorMessage = "Debe proporcionar un correo electrónico válido.")] string Correo,
        [property: Required(ErrorMessage = "El teléfono es obligatorio.")] string Telefono,
        string Direccion);
}
