using Dominio.Dummies;
namespace Dominio.Interfaces;

public interface IGestorRecursos
{
    public void agregarRecurso(Usuario solicitante, Recurso recurso);
    public void eliminarRecurso(int id, List<Proyecto> proyectos);
    public List<Recurso> recursosPorProyecto(Proyecto proyecto);
    // falta modificacion
   
}