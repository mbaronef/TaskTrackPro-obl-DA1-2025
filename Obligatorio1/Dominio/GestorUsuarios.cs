using System.Security.Cryptography;
using System.Text;
using Dominio.Excepciones;

namespace Dominio;

public class GestorUsuarios
{
    private static int _cantidadUsuarios;
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public List<Usuario> Usuarios { get; private set; } = new List<Usuario>();

    public GestorUsuarios()
    {
    }

    public void AgregarUsuario(Usuario usuario)
    {
        _cantidadUsuarios++;
        usuario.Id = _cantidadUsuarios;
        Usuarios.Add(usuario);
    }

    public void EliminarUsuario(int id)
    {
        Usuarios.RemoveAll(u => u.Id == id);
    }

    public Usuario ObtenerUsuario(int idUsuario)
    {
        return Usuarios.Find(u => u.Id == idUsuario);
    }

    public void AgregarAdministradorSistema(int idUsuario)
    {
        Usuario usuario = Usuarios.Find(u => u.Id == idUsuario);
        usuario.EsAdministradorSistema = true;
    }

    public void EliminarAdministradorSistema(int idUsuario)
    {
        Usuario usuario = Usuarios.Find(u => u.Id == idUsuario);
        usuario.EsAdministradorSistema = false;
    }

    public void AsignarAdministradorProyecto(Usuario solicitante, Usuario nuevoAdministradorProyecto)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para asignar administradores de proyectos.");
        }
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
    }

    public void EliminarAdministradorProyecto(Usuario solicitante, Usuario administradorProyecto)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para eliminar administradores de proyectos.");
        }

        if (!administradorProyecto.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("El usuario a eliminar no es administrador de proyectos.");
        }
        administradorProyecto.EsAdministradorProyecto = false;
    }

    public void ReiniciarContrasena(Usuario administrador, Usuario usuarioObjetivo)
    {
        if (!administrador.EsAdministradorSistema && !administrador.EsAdministradorProyecto)
        {
            throw new ExcepcionDominio("No tiene los permisos necesarios para reiniciar la contraseña del usuario.");
        }
        usuarioObjetivo.CambiarContrasena("TaskTrackPro@2025");
    }

    public string AutogenerarContrasena(Usuario administrador, Usuario usuarioObjetivo)
    {
        string minusculas = "abcdefghijklmnopqrstuvwxyz";
        string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numeros = "1234567890";
        string simbolos = "!@#$%^&*()-_=+<>?{}[].,:;¡¿?'/~|°";
        string todosLosCaracteres = minusculas + mayusculas + numeros + simbolos;
        
        StringBuilder contrasenaAutogenerada = new StringBuilder();
        Random random = new Random();
        
        int largo = random.Next(8, 16); // el largo es un número random entre 8 y 15
        
        // agregar manualmente una mayúscula, una minúscula, un número y un caracter especial (para asegurar restricciones de contraseña)
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(minusculas, random));
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(mayusculas, random));
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(numeros, random));
        contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(simbolos, random));

        while (contrasenaAutogenerada.Length < largo)
            contrasenaAutogenerada.Append(ObtenerCaracterAleatorio(todosLosCaracteres, random)); // agrega a la contraseña caracteres random hasta cumplir longitud

        string nuevaContrasena = MezclarCaracteres(contrasenaAutogenerada.ToString(), random);
        usuarioObjetivo.CambiarContrasena(nuevaContrasena);
        return nuevaContrasena; // devuelve la contraseña en texto plano para mostrársela una vez al usuario cuando se genera
    }

    private static char ObtenerCaracterAleatorio(string caracteres, Random rand) {
        int indice = rand.Next(0, caracteres.Length);
        return caracteres[indice]; 
    } 
    private static string MezclarCaracteres(string input, Random rand) {
        char[] array = input.ToCharArray(); // se convierte en array para poder recorrer cada caracter
        for (int i = array.Length - 1; i > 0; i--) // shuffle
        {
            int j = rand.Next(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array);
    }


    // refactor :private string GenerarContrasenaValida() y usar RandomNumberGenerator
}