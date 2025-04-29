using Dominio.Dummies;

namespace IGestores;

public interface IGestorUsuarios : IGestor<Usuario>
{
    Usuario ObtenerUsuarioPorID(int id);
    void AsignarAdministradorSistema(Usuario solicitante, int idUsuarioObjetivo);
    void AsignarAdministradorProyecto(Usuario solicitante, int idUsuarioObjetivo);
    void EliminarAdministradorProyecto(Usuario solicitante, int idUsuarioObjetivo);
    void ReiniciarContrasena(Usuario solicitante, int idUsuarioObjetivo);
    void AutogenerarContrasena(Usuario solicitante, int idUsuarioObjetivo);
    void ModificarContrasena(Usuario solicitante, int idUsuarioObjetivo, string nuevaContrasena);
    Usuario Login(string email, string contrasena);
    // Podrían haberse separado una interfaz para gestión de permisos, una para gestión de contraseñas y una para autenticación
    // Se decidió no hacerlo en esta oportunidad y darle a las clases que implementen la interfaz la responsabilidad total de gestionar usuarios.
}
