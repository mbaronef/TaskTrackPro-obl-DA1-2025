using System.ComponentModel.DataAnnotations;

namespace DTOS_;

public class ModeloSesionDTO
{
    [EmailAddress] public string Email { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
}