using System.Text;
using System.Text.Json;
using IRepositorios;
using IServicios;

namespace Servicios.Exportacion;

public class ExportadorJson : IExportadorProyectos
{
    private readonly IRepositorioProyectos _repositorio;

    public ExportadorJson(IRepositorioProyectos repositorio)
    {
        _repositorio = repositorio;
    }

    public string NombreFormato => "json";
    public string TipoContenido => "application/json";
    public string NombreArchivo => "proyectos.json";

    public Task<byte[]> Exportar()
    {
        var proyectos = _repositorio.ObtenerTodos();

        var opciones = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        var json = JsonSerializer.Serialize(proyectos, opciones);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Task.FromResult(bytes);
    }
}