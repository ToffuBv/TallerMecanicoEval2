using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO;

public record RegistroRequest(
    [property: Required(ErrorMessage = "El nombre es obligatorio.")][property: MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")] string Nombre,
    [property: Required(ErrorMessage = "El correo es obligatorio.")][property: EmailAddress(ErrorMessage = "Debe proporcionar un correo electrónico válido.")] string Correo,
    [property: Required(ErrorMessage = "La contraseña es obligatoria.")][property: MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")] string Password);
