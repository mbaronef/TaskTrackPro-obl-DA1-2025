namespace Dominio;

public class Usuario
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Contrasena { get; set; }
    public string Email { get; set; }

    public Usuario(string unNombre, string unApellido, string unContrasena, string unEmail)
    {
        Nombre = unNombre;
        Apellido = unApellido;
        Contrasena = unContrasena;
        Email = unEmail;
    }
}