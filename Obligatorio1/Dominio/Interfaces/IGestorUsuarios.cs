using Dominio.Dummies;

namespace Dominio.Interfaces
{
    public interface IGestorUsuarios
    {
        void agregarUsuario(Usuario usuario);
        void eliminarUsuario(int id);
        void asignarContrasenaPorDefecto(Usuario administrador, Usuario usuario);
        string reiniciarContrasena(Usuario administrador, Usuario usuario); // no se pueden hacer metodos privates en interfaces
        void login(string email, string contrasena);
        void asignarAdministradorProyecto(Usuario solicitante, Usuario nuevoAdministradorProyecto);
        //falta modificacion
    }
}