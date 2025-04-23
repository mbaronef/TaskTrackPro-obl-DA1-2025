namespace Dominio;

public class Usuario
{
    public static int Contador { get; set; }
    
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Contrasena { get; set; }
    public string Email { get; set; }

    public Usuario(string unNombre, string unApellido, string unContrasena, string unEmail)
    {
        Contador++;
        Id = Contador;
        Nombre = unNombre;
        Apellido = unApellido;
        Contrasena = unContrasena;
        Email = unEmail;
    }

    public bool contrasenaValida()
    {
        return this.Contrasena.Any(char.IsUpper);
    }
}