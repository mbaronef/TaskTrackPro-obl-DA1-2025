using Dominio;

namespace Servicios.Exportacion;

public interface IExportadorProyectos
{
    string NombreFormato { get; }
    string ContentType { get; }
    string NombreArchivo { get; } 
    
    Task<byte[]> Exportar(); 
}