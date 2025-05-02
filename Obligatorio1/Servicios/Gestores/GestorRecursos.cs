using Dominio;

namespace Servicios.Gestores;

public class GestorRecursos
{
    public List<Recurso> Recursos { get; private set; }

    public GestorRecursos()
    {
        Recursos =  new List<Recurso>();
    }


}