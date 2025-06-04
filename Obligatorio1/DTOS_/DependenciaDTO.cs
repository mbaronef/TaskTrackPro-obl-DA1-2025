using System.ComponentModel.DataAnnotations;

namespace DTOS_;

public class DependenciaDTO
{
    [Required(ErrorMessage = "Seleccione una tarea previa.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una tarea previa.")] 
    public int TareaPreviaId { get; set; }

    [Required(ErrorMessage = "Seleccione un tipo de dependencia.")]
    [RegularExpression("^(SS|FS)$", ErrorMessage = "El tipo de dependencia debe ser 'SS' o 'FS'.")]
    public string Tipo { get; set; }
}