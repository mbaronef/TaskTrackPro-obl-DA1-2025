using Dominio;
using Servicios.Excepciones;

namespace Servicios.Utilidades;

public static class CaminoCritico
{ 
    public static void CalcularCaminoCritico(Proyecto proyecto)
    {
        if (ProyectoTieneTareas(proyecto))
        {
            List<Tarea> tareas = proyecto.Tareas;
            List<Tarea> tareasOrdenTopologico = OrdenarTopologicamente(tareas);

            foreach (Tarea tarea in tareasOrdenTopologico)
            {
                if (!tarea.Dependencias.Any())
                {
                    tarea.ModificarFechaInicioMasTemprana(proyecto.FechaInicio);
                }
                else
                {
                    CalcularFechasMasTempranas(tarea);
                }
            }

            proyecto.FechaFinMasTemprana = tareas.Max(t => t.FechaFinMasTemprana);
            proyecto.NotificarMiembros(
                $"Se cambió la fecha de fin más temprana del proyecto '{proyecto.Nombre}' a '{proyecto.FechaFinMasTemprana:dd/MM/yyyy}'.");

            Dictionary<Tarea, List<Tarea>> sucesoras = ObtenerSucesorasPorTarea(tareas);
            CalcularHolguras(tareasOrdenTopologico, sucesoras, proyecto);
        }
    }
    
    private static List<Tarea> OrdenarTopologicamente(List<Tarea> tareas)
    {
        List<Tarea> tareasOrdenadas = new List<Tarea>();

        Dictionary<Tarea, int> gradosDeEntrada = new Dictionary<Tarea, int>();
        Dictionary<Tarea, List<Tarea>> sucesoras = new Dictionary<Tarea, List<Tarea>>();
        InicializarGradosDeEntradaYSucesoras(tareas, gradosDeEntrada, sucesoras);
        
        Queue<Tarea> cola = new Queue<Tarea>(tareas.Where(t => gradosDeEntrada[t] == 0));
        while (cola.Any())
        {
            Tarea tareaActual = cola.Dequeue();
            tareasOrdenadas.Add(tareaActual);
            
            foreach (Tarea siguiente in sucesoras[tareaActual])
            {
                gradosDeEntrada[siguiente]--;
                if (gradosDeEntrada[siguiente] == 0)
                {
                    cola.Enqueue(siguiente);
                }
            }
        }
        
        if (tareasOrdenadas.Count != tareas.Count)
        { // Validación: si hay ciclo, no se procesaron todas
            throw new ExcepcionServicios("El grafo de tareas tiene dependencias cíclicas.");
        }
        
        return tareasOrdenadas;
    }
    
    private static void InicializarGradosDeEntradaYSucesoras(List<Tarea> tareas,  Dictionary<Tarea, int> gradosDeEntrada, Dictionary<Tarea, List<Tarea>> sucesoras)
    {
        foreach (Tarea tarea in tareas)
        {
            gradosDeEntrada[tarea] = 0;
            sucesoras[tarea] = new List<Tarea>();
        }
        foreach (Tarea tarea in tareas)
        {
            foreach (Dependencia dependencia in tarea.Dependencias)
            {
                Tarea anterior = dependencia.Tarea;
                gradosDeEntrada[tarea]++;
                sucesoras[anterior].Add(tarea);
            }
        }
    }
    
    private static void CalcularFechasMasTempranas(Tarea tarea)
    {
        List<DateTime> fechas = new List<DateTime>();
        
        foreach (Dependencia dependencia in tarea.Dependencias)
        {
            Tarea tareaAnterior = dependencia.Tarea;

            if (dependencia.Tipo == "FS")
            {
                fechas.Add(tareaAnterior.FechaFinMasTemprana.AddDays(1));
            }
            else if (dependencia.Tipo == "SS")
            {
                fechas.Add(tareaAnterior.FechaInicioMasTemprana);
            }
        }
        tarea.ModificarFechaInicioMasTemprana(fechas.Max());
    }

    private static Dictionary<Tarea, List<Tarea>> ObtenerSucesorasPorTarea(List<Tarea> tareas)
    {
        Dictionary<Tarea, List<Tarea>> sucesoras = new Dictionary<Tarea, List<Tarea>>();

        foreach (Tarea tarea in tareas)
        {
            sucesoras[tarea] = new List<Tarea>();
        }

        foreach (Tarea tarea in tareas)
        {
            foreach (Dependencia dependencia in tarea.Dependencias)
            {
                Tarea tareaPredecesora = dependencia.Tarea;
                sucesoras[tareaPredecesora].Add(tarea);
            }
        }
        return sucesoras;
    }
    
    private static void CalcularHolguras(List<Tarea> tareasOrdenTopologico, Dictionary<Tarea, List<Tarea>> sucesoras,
        Proyecto proyecto)
    {
        for (int i = tareasOrdenTopologico.Count - 1; i >= 0; i--)
        {
            Tarea tarea = tareasOrdenTopologico[i];
            DateTime fechaLimite = ObtenerFechaLimite(tarea, sucesoras, proyecto.FechaFinMasTemprana);
            
            int holgura = (int)(fechaLimite - tarea.FechaFinMasTemprana).TotalDays;
            tarea.Holgura = Math.Max(0, holgura);
        }
    }
    
    private static DateTime ObtenerFechaLimite(Tarea tarea, Dictionary<Tarea, List<Tarea>> sucesoras, DateTime fechaFinProyecto)
    {
        if (!sucesoras[tarea].Any())
        {
            return fechaFinProyecto;
        }

        List<DateTime> posiblesLimites = sucesoras[tarea]
            .SelectMany(sucesora => sucesora.Dependencias
                .Where(dep => dep.Tarea == tarea)
                .Select(dep => dep.Tipo == "FS"
                    ? sucesora.FechaInicioMasTemprana.AddDays(-1)
                    : sucesora.FechaInicioMasTemprana))
            .ToList();
        return posiblesLimites.Min();
    }
    
    private static bool ProyectoTieneTareas(Proyecto proyecto)
    {
        return proyecto.Tareas.Any();
    }
}