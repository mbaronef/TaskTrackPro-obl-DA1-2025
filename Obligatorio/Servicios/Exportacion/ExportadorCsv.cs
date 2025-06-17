using System.Text;
using IRepositorios;
using IServicios;

namespace Servicios.Exportacion;

public class ExportadorCsv : IExportadorProyectos
{
    private readonly IRepositorioProyectos _repositorio;
    public ExportadorCsv(IRepositorioProyectos repositorio)
    {
        _repositorio = repositorio;
    }

    public string NombreFormato => "csv";
    public string TipoContenido => "text/csv";
    public string NombreArchivo => "proyectos.csv";

    public Task<byte[]> Exportar()
    {
        var proyectos = _repositorio.ObtenerTodos();

        var sb = new StringBuilder();
        sb.AppendLine("Id,Nombre,Descripcion,FechaInicio"); // Cambiar según lo que necesito tener en el archivo por proyecto

        foreach (var p in proyectos)
        {
            sb.AppendLine($"{p.Id},\"{p.Nombre}\",\"{p.Descripcion}\",{p.FechaInicio:yyyy-MM-dd}");
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
    
}
