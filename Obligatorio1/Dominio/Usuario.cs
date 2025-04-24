namespace Dominio;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public string Contrasena { get; set; }
    public List<Notificacion> Notificaciones { get; private set; } = new List<Notificacion>();
    public bool EsAdministradorProyecto { get; set; } =  false;
    

    public Usuario(string unNombre, string unApellido, DateTime unaFechaNacimiento, string unEmail, string unaContrasena)
    {
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