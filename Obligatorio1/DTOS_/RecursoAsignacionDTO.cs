using System.ComponentModel.DataAnnotations;

namespace DTOS_;

public class RecursoAsignacionDTO
{
    [Required(ErrorMessage = "Debe seleccionar un recurso.")]
    public int Id { get; set; }
}