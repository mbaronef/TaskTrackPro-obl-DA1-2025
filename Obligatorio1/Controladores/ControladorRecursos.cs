using DTOs;
using Servicios.Gestores.Interfaces;

namespace Controladores;

public class ControladorRecursos
{
    private IGestorRecursos _gestorRecursos;
    
    public ControladorRecursos(IGestorRecursos gestorRecursos)
    {
        _gestorRecursos = gestorRecursos;
    }
    
    public void AgregarRecurso(UsuarioDTO solicitanteDTO, RecursoDTO nuevoRecursoDTO, bool esExclusivo)
    {
        _gestorRecursos.AgregarRecurso(solicitanteDTO, nuevoRecursoDTO, esExclusivo);
    }
}