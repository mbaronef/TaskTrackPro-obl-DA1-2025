using Dominio.Dummies;

namespace Repositorios.IRepositorios;

public interface IRepositorioProyectos : IRepositorio<Proyecto>
{ 
  void ModificarNombre(int idProyecto, string nombre);
  void ModificarDescripcion(int idProyecto, string descripcion);
  void ModificarFechaInicio(int idProyecto, DateTime fechaInicio);
  void ModificarFechaFinMasTemprana(int idProyecto, DateTime fechaFinMasTemprana);
  void ModificarAdministrador(int idProyecto, Usuario administrador);
  void AgregarTarea(int idProyecto, Tarea tarea);
  void EliminarTarea(int idProyecto, int idTarea);
  void AgregarMiembro(int idProyecto, Usuario miembro);
  void EliminarMiembro(int idProyecto, int idMiembro);
  
  
}