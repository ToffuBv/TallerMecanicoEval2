using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.ValidacionesDTO
{
    public record ModeloDTO(
        [property: Required(ErrorMessage = "El nombre del modelo es obligatorio.")][property: StringLength(100)] string Nombre,
        [property: Range(1, int.MaxValue, ErrorMessage = "Debe asignar una marca válida.")] int MarcaId);
}
