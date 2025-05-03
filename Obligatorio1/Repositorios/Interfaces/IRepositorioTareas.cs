using Dominio.Dummies;

namespace Repositorios.IRepositorios;

public interface IRepositorioTareas : IRepositorio<Tarea>
{
    void ModificarTitulo(int idTarea, string titulo);
    void ModificarDescripcion(int idTarea, string descripcion);
    void ModificarDuracion(int idTarea, int duracion);
    void ModificarFechaInicioMasTemprana(int idTarea, DateTime fechaInicioMasTemprana);
    void ModificarFechaFinMasTemprana(int idTarea, DateTime fechaFinMasTemprana);
    void ModificarFechaDeEjecucion(int idTarea, DateTime fechaDeEjecucion);
    void ModificarEstado(int idTarea, EstadoTarea estado);
    void ModificarHolgura(int idTarea, int holgura);
    void AgregarUsuario(int idTarea, Usuario usuarioAsignado);
    void EliminarUsuario(int idTarea, int idUsuarioAsignado);
    void AgregarRecursoNecesario(int idTarea, Recurso recursoNecesario);
    void EliminarRecursoNecesario(int idTarea, Recurso recursoNecesario);
    void AgregarDependencia(int idTarea, Dependencia dependencia);
    void EliminarDependencia(int idTarea, Dependencia dependencia);
}