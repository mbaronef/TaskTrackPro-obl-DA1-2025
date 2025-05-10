using Dominio;

namespace Interfaz.ServiciosInterfaz
{
    public class UsuarioActual
    {
        public Usuario UsuarioLogueado { get; private set; }

        public void EstablecerUsuario(Usuario usuario)
        {
            UsuarioLogueado = usuario;
        }

        public void CerrarSesion()
        {
            UsuarioLogueado = null;
        }
    }
}