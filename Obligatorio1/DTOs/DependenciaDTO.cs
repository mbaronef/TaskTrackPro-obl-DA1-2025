using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class DependenciaDTO
{
    [Required(ErrorMessage = "Seleccione una tarea previa.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una tarea previa.")]
    public int TareaPreviaId { get; set; }

    public TareaDTO TareaPrevia { get; set; }

    [Required(ErrorMessage = "Seleccione un tipo de dependencia.")]
    [RegularExpression("^(SS|FS)$", ErrorMessage = "El tipo de dependencia debe ser 'SS' o 'FS'.")]
    public string Tipo { get; set; }

    public static DependenciaDTO DesdeEntidad(Dependencia dependencia)
    {
        return new DependenciaDTO
        {
            TareaPrevia = TareaDTO.DesdeEntidad(dependencia.Tarea),
            TareaPreviaId = dependencia.Tarea.Id,
            Tipo = dependencia.Tipo.ToString()
        };
    }
}