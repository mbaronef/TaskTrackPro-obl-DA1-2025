using System.Text;
using System.Text.Json;
using Dominio;
using Repositorios.Interfaces;
using Servicios.Exportacion;

namespace Servicios.Exportacion;

public class ExportadorJson : IExportadorProyectos
{
    private readonly IRepositorio<Proyecto> _repositorio; // Cambiar luego por IRepositorioProyectos

    public ExportadorJson(IRepositorio<Proyecto> repositorio)
    {
        _repositorio = repositorio;
    }

    public string NombreFormato => "json";
    public string ContentType => "application/json";
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