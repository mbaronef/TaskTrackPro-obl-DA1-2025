using System.Text.RegularExpressions;
using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Usuario
{
    private string _contrasenaEncriptada;
    private static readonly int _edadMinima = 18;
    private static readonly int _edadMaxima = 100;

    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public List<Notificacion> Notificaciones { get; private set; }
    public bool EsAdministradorSistema { get; set; } = false;

    public bool EsAdministradorProyecto { get; set; } = false;
    
    public bool EstaAdministrandoUnProyecto { get; set; } = false;

    public int CantidadProyectosAsignados { get; set; } = 0;

    public Usuario()
    { 
        Notificaciones = new List<Notificacion>();
    }

    public Usuario(string nombre, string apellido, DateTime fechaNacimiento, string email, string contrasenaEncriptada)
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        ValidarAtributoNoVacio(apellido, "apellido");
        ValidarEdad(fechaNacimiento);
        ValidarEmail(email);
        
        Nombre = nombre;
        Apellido = apellido;
        FechaNacimiento = fechaNacimiento;
        Email = email;
        _contrasenaEncriptada = contrasenaEncriptada;
        Notificaciones = new List<Notificacion>();
    }

    public void EstablecerContrasenaEncriptada(string contrasenaEncriptada)
    {
        _contrasenaEncriptada = contrasenaEncriptada;
    }

    public bool Autenticar(string contrasenaIngresada)
    {
        return BCrypt.Net.BCrypt.Verify(contrasenaIngresada, _contrasenaEncriptada);
    }
    
    public void CambiarEmail(string nuevoEmail)
    {
        ValidarEmail(nuevoEmail);
        Email = nuevoEmail;
    }

    public void RecibirNotificacion(string mensaje)
    {
        Notificacion notificacion = new Notificacion(mensaje);
        Notificaciones.Add(notificacion);
    }

    public void BorrarNotificacion(int idNotificacion)
    {
        int indice = Notificaciones.FindIndex(n => n.Id == idNotificacion);
        if (indice == -1)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.NotificacionNoExiste);
        }
        Notificaciones.RemoveAt(indice);

    }
    
    private void ValidarAtributoNoVacio(string texto, string nombreAtributo)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.AtributoVacio, nombreAtributo));   
        }
    }
    
    private void ValidarEdad(DateTime fechaNacimiento)
    {
        if (fechaNacimiento.AddYears(_edadMinima) > DateTime.Today)
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.EdadMinima, _edadMinima));
        }

        if (fechaNacimiento.AddYears(_edadMaxima) <= DateTime.Today)
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.EdadMaxima, _edadMaxima));
        }
    }

    private void ValidarEmail(string email)
    {
        // Valida formato de email con arroba y dominio (ej: usuario@dominio.com)
        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            throw new ExcepcionDominio(MensajesErrorDominio.EmailInvalido);
        }
    }

    public override bool Equals(object? otro)
    {
        Usuario otroUsuario = otro as Usuario;
        return otroUsuario != null && Id == otroUsuario.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Nombre} {Apellido} ({Email})";
    }
}