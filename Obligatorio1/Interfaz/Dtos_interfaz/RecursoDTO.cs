using System.ComponentModel.DataAnnotations;

namespace Interfaz.DTOs;

public class RecursoDTO
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }
        
    [Required(ErrorMessage = "El tipo es obligatorio.")]
    public string Tipo { get; set; }
        
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; }
}