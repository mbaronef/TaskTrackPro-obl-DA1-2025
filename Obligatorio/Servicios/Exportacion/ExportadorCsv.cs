using System.Text;
using System.Text.Json;
using Dominio;
using Repositorios.Interfaces;

namespace Servicios.Exportacion;

public class ExportadorCsv : IExportadorProyectos
{
    private readonly IRepositorio<Proyecto> _repositorio; // Cambiar luego por IRepositorioProyectos

    public ExportadorCsv(IRepositorio<Proyecto> repositorio)
    {
        _repositorio = repositorio;
    }

    public string NombreFormato => "csv";
    public string ContentType => "text/csv";
    public string NombreArchivo => "proyectos.csv";

    public Task<byte[]> Exportar()
    {
        var proyectos = _repositorio.ObtenerTodos();

        var sb = new StringBuilder();
        sb.AppendLine("Id,Nombre,Descripcion,FechaInicio");

        foreach (var p in proyectos)
        {
            sb.AppendLine($"{p.Id},\"{p.Nombre}\",\"{p.Descripcion}\",{p.FechaInicio:yyyy-MM-dd}");
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
    
}
