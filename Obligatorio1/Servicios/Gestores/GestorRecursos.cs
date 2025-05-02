using Dominio;
using Dominio.Dummies;

namespace Servicios.Gestores;

public class GestorRecursos
{
    public List<Recurso> Recursos { get; private set; }

    public GestorRecursos()
    {
        Recursos =  new List<Recurso>();
    }

    public void AgregarRecurso(Usuario solicitante, Recurso recurso)
    {
        if (!solicitante.EsAdministradorSistema)
        {
            throw new ExcepcionServicios("No tiene los permisos necesarios para agregar recursos");
        }
        Recursos.Add(recurso);
    }

}