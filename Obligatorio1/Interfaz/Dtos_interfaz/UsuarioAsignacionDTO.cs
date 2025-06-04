using System.ComponentModel.DataAnnotations;

namespace Interfaz.DTOs;

public class UsuarioAsignacionDTO
{
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    public int Id { get; set; }
}