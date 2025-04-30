using System.Security.Cryptography;
using System.Text;
using Dominio.Excepciones;

namespace Dominio;

public class GestorUsuarios
{
    private static int _cantidadUsuarios;
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    private static readonly int _largoMinContrasena = 8;
    private static readonly int _largoMaxContrasena = 15;
    public List<Usuario> Usuarios { get; } = new List<Usuario>();

    public GestorUsuarios(Usuario adminSistema)
    {
        Usuarios.Add(adminSistema);
        adminSistema.EsAdministradorSistema = true;
        //No se manejan ids, el primer administrador tiene id 0
    }

    public void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        VerificarPermisoAdminSistema(solicitante, "crear usuarios");
        usuario.Id = ++_cantidadUsuarios;
        Usuarios.Add(usuario);
        string mensajeNotificacion =
            $"Se creó un nuevo usuario: {usuario.Nombre} {usuario.Apellido}";
        NotificarAdministradoresDeSistema(solicitante, mensajeNotificacion);
    }

    private void VerificarPermisoAdminSistema(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionDominio($"No tiene los permisos necesarios para {accion}");
        }
    }

    private void NotificarAdministradoresDeSistema(Usuario solicitante, string mensajeNotificacion)
    {
        List<Usuario> administradores = Usuarios.Where(u => u.EsAdministradorSistema && !u.Equals(solicitante)).ToList();
        foreach (Usuario admin in administradores)
        {
            Notificar(admin, mensajeNotificacion);
        }
    }

    public void EliminarUsuario(Usuario solicitante, int id)
    {
        if (id == 0)
        {
            throw new ExcepcionDominio("No se puede eliminar al primer administrador del sistema");
        }
        Usuario usuario = ObtenerUsuarioPorId(id);
        if (!solicitante.EsAdministradorSistema && !solicitante.Equals(usuario))
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para eliminar usuarios");
        }
        Usuarios.Remove(usuario);
        string mensajeNotificacion = $"Se eliminó un nuevo usuario. Nombre: {usuario.Nombre}, Apellido: {usuario.Apellido}";
        NotificarAdministradoresDeSistema(solicitante, mensajeNotificacion);
    }

    public Usuario ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = Usuarios.SingleOrDefault(u => u.Id == idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionDominio("El usuario no existe");
        }
        return usuario;
    }

    public void AgregarAdministradorSistema(Usuario solicitante, int idUsuario)
    {
        VerificarPermisoAdminSistema(solicitante, "asignar un administrador de sistema");
        Usuario usuario = ObtenerUsuarioPorId(idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void AsignarAdministradorProyecto(Usuario solicitante, int idUsuario)
    {
        VerificarPermisoAdminSistema(solicitante, "asignar administradores de proyecto");
        Usuario nuevoAdministradorProyecto = ObtenerUsuarioPorId(idUsuario);
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void DesasignarAdministradorProyecto(Usuario solicitante, int idUsuario)
    {
        VerificarPermisoAdminSistema(solicitante, "desasignar administradores de proyecto");
        Usuario administradorProyecto = ObtenerUsuarioPorId(idUsuario);
        if (!administradorProyecto.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("El usuario a desasignar no es administrador de proyectos.");
        }
        if (administradorProyecto.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionDominio("No se puede quitar permisos de proyecto a un usuario que tiene un proyecto a su cargo.");
        }
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto &&
            !solicitante.Equals(usuarioObjetivo))
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para reiniciar la contraseña del usuario");
        }
        usuarioObjetivo.EstablecerContrasena(_contrasenaPorDefecto);
        Notificar(usuarioObjetivo, $"Se reinició su contraseña. La nueva contraseña es {_contrasenaPorDefecto}");
    }

    public void AutogenerarContrasena(Usuario solicitante, int idUsuarioObjetivo)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para autogenerar la contraseña del usuario");
        }
        string nuevaContrasena = GenerarContrasenaValida();
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasena(nuevaContrasena);
        Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
    }

    private static string GenerarContrasenaValida()
    {
        string minusculas = "abcdefghijklmnñopqrstuvwxyz";
        string mayusculas = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        string numeros = "1234567890";
        string simbolos = "!@#$%^&*()-_=+<>?{}[].,:;¡¿?'/~|°";
        string todosLosCaracteres = minusculas + mayusculas + numeros + simbolos;
        
        StringBuilder contrasenaAutogenerada = new StringBuilder();
        
        RandomNumberGenerator generadorDeNumerosAleatorio = RandomNumberGenerator.Create(); // generador de números aleatorios criptográficamente seguros
        
        int largo = GenerarNumeroAleatorio(_largoMinContrasena, _largoMaxContrasena, generadorDeNumerosAleatorio); // el largo es un número random entre 8 y 15
        // agregar manualmente una mayúscula, una minúscula, un número y un caracter especial (para asegurar restricciones de contraseña)
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(minusculas, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(mayusculas, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(numeros, generadorDeNumerosAleatorio));
        contrasenaAutogenerada.Append(GenerarCaracterAleatorio(simbolos, generadorDeNumerosAleatorio));
            
        while (contrasenaAutogenerada.Length < largo)
            contrasenaAutogenerada.Append(GenerarCaracterAleatorio(todosLosCaracteres, generadorDeNumerosAleatorio)); // agrega a la contraseña caracteres random hasta cumplir longitud
        return MezclarCaracteres(contrasenaAutogenerada.ToString(), generadorDeNumerosAleatorio);
    }

    private static int GenerarNumeroAleatorio(int min, int max, RandomNumberGenerator rng)
    {
        byte[] buffer = new byte[sizeof(uint)]; // uint: entero de 32 bits sin signo. En el buffer se almacena un número aleatorio.
        rng.GetBytes(buffer); // llena el buffer con números aleatorios
        uint numero = BitConverter.ToUInt32(buffer, 0); // se convierte el buffer en número de tipo uint
        // asegurar que el número esté en el rango
        int rango = max - min + 1;
        return (int)(numero % rango) + min;
    }
    private static char GenerarCaracterAleatorio(string caracteres, RandomNumberGenerator rng)
    {
        int indice = GenerarNumeroAleatorio(0, caracteres.Length - 1, rng);
        return caracteres[indice];
    }
    private static string MezclarCaracteres(string input, RandomNumberGenerator rng)
    {
        char[] array = input.ToCharArray(); // Convierte la cadena en un array de caracteres para poder recorrerla char a char
        for (int i = array.Length - 1; i > 0; i--)
        { //shuffle
            int j = GenerarNumeroAleatorio(0, i, rng);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array); // Convierte el array de nuevo a una cadena
    }

    public void ModificarContrasena(Usuario solicitante, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario usuarioObjetivo = ObtenerUsuarioPorId(idUsuarioObjetivo);
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto &&
            !solicitante.Equals(usuarioObjetivo))
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para modificar la contraseña del usuario");
        }
        usuarioObjetivo.EstablecerContrasena(nuevaContrasena);
        if (!solicitante.Equals(usuarioObjetivo))
        {
            Notificar(usuarioObjetivo, $"Se modificó su contraseña. La nueva contraseña es {nuevaContrasena}");
        }
    }
    
    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        usuario.RecibirNotificacion(mensajeNotificacion);
    }

    public Usuario LogIn(string email, string contrasena)
    {
        Usuario usuario = Usuarios.FirstOrDefault(u => u.Email == email);
        if (usuario == null)
        {
            throw new ExcepcionDominio("Correo electrónico no registrado.");
        }
        if (!usuario.Autenticar(contrasena))
        {
            throw new ExcepcionDominio("La contraseña ingresada es incorrecta.");
        }
        return usuario;
    }
}