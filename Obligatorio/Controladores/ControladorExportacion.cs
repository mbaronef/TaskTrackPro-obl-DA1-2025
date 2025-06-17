using DTOs;
using Excepciones;
using Excepciones.MensajesError;
using Servicios.Exportacion;

namespace Controladores;

public class ControladorExportacion
{
    private readonly IEnumerable<IExportadorProyectos> _exportadores;

    public ControladorExportacion(IEnumerable<IExportadorProyectos> exportadores)
    {
        _exportadores = exportadores;
    }

    public async Task<ArchivoExportadoDTO> Exportar(string formato)
    {
        var exportador = _exportadores
            .FirstOrDefault(e => e.NombreFormato.Equals(formato, StringComparison.OrdinalIgnoreCase));

        if (exportador == null)
            throw new ExcepcionExportador(MensajesErrorServicios.FormatoNoSoportado);

        byte[] contenido = await exportador.Exportar();
        return new ArchivoExportadoDTO(exportador.TipoContenido, exportador.NombreArchivo, contenido);
    }
}