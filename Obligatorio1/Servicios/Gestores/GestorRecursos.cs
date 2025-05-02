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

    public void AgregarRecurso(Usuario usuario, Recurso recurso)
    {
        Recursos.Add(recurso);
    }

}