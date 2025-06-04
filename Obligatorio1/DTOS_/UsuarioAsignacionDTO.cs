using System.ComponentModel.DataAnnotations;

namespace DTOS_;

public class UsuarioAsignacionDTO
{
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    public int Id { get; set; }
}