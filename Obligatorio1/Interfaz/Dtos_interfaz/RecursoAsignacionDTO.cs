using System.ComponentModel.DataAnnotations;

namespace Interfaz.DTOs;

public class RecursoAsignacionDTO
{
    [Required(ErrorMessage = "Debe seleccionar un recurso.")]
    public int Id { get; set; }
}