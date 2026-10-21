using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO;

public record AdministrarUsuarioDTO(
    [property: Required, StringLength(100, MinimumLength = 3)] string Nombre,
    [property: Required, EmailAddress, StringLength(150)] string Correo,
    [property: Range(1, int.MaxValue)] int RolId,
    [property: StringLength(200, MinimumLength = 6)] string? Password);
