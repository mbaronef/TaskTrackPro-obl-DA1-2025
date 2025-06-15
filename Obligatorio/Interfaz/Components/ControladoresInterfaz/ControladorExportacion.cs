using Microsoft.AspNetCore.Mvc;
using Servicios.Exportacion;

namespace Interfaz.Components.ControladoresInterfaz;

[Route("exportar")]
public class ControladorExportacion : Controller
{
    private readonly IEnumerable<IExportadorProyectos> _exportadores;

    public ControladorExportacion(IEnumerable<IExportadorProyectos> exportadores)
    {
        _exportadores = exportadores;
    }

    [HttpGet("{formato}")]
    public async Task<IActionResult> Exportar(string formato)
    {
        var exportador = _exportadores
            .FirstOrDefault(e => e.NombreFormato.Equals(formato, StringComparison.OrdinalIgnoreCase));

        if (exportador == null)
            return BadRequest($"No hay exportador para el formato '{formato}'");

        var contenido = await exportador.Exportar();
        return File(contenido, exportador.ContentType, exportador.NombreArchivo);
    }
}
