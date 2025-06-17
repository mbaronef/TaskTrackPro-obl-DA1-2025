namespace Servicios.Exportacion;

public interface IExportadorProyectosFactory
{
    public IExportadorProyectos CrearExportador(string formato);
}