namespace Dominio;

public class Usuario
{
    public static int Contador { get; set; }
    
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public string Contrasena { get; set; }
    

    public Usuario(string unNombre, string unApellido, DateTime unaFechaNacimiento, string unEmail, string unaContrasena)
    {
        Contador++;
        Id = Contador;
        Nombre = unNombre;
        Apellido = unApellido;
        FechaNacimiento = unaFechaNacimiento;
        Email = unEmail;
        Contrasena = unaContrasena;
    }

    public bool contrasenaValida()
    {
        return this.Contrasena.Any(char.IsUpper);
    }
}