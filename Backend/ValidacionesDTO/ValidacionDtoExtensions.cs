using MiniValidation;

namespace TallerMecanico.ValidacionesDTO;

public static class ValidacionDtoExtensions
{
    public static IResult? Validar(this object dto)
    {
        return MiniValidator.TryValidate(dto, out var errores)
            ? null
            : Results.ValidationProblem(errores);
    }
}
