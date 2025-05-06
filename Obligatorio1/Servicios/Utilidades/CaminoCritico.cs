using Dominio;
using Dominio.Excepciones;

namespace Servicios.Utilidades;

public static class CaminoCritico
{
    public static void CalcularCaminoCritico(Proyecto proyecto)
    {
        List<Tarea> tareas = proyecto.Tareas;
        List<Tarea> tareasOrdenadas = ordenarTopologicamente(tareas);
        
        foreach (Tarea tarea in tareasOrdenadas)
        {
            if (!tarea.Dependencias.Any())
            {
                tarea.FechaInicioMasTemprana = proyecto.FechaInicio;
            }
            else
            {
                DateTime maxFecha = DateTime.MinValue;

                foreach (Dependencia dependencia in tarea.Dependencias)
                {
                    Tarea anterior = dependencia.Tarea;

                    if (dependencia.Tipo == "FS")
                    {
                        maxFecha = DateTime.Compare(maxFecha, anterior.FechaFinMasTemprana) > 0 ? maxFecha : anterior.FechaFinMasTemprana;
                    }
                    else if (dependencia.Tipo == "SS")
                    {
                        maxFecha = DateTime.Compare(maxFecha, anterior.FechaInicioMasTemprana) > 0 ? maxFecha : anterior.FechaInicioMasTemprana;
                    }
                }

                tarea.FechaInicioMasTemprana = maxFecha;
            }
        }
        
        proyecto.FechaFinMasTemprana = tareas.Max(t => t.FechaFinMasTemprana);
        
        Dictionary<Tarea, List<Tarea>> sucesoras = MapearSucesoras(tareas);

        for (int i = tareasOrdenadas.Count - 1; i >= 0; i--) 
        {
            Tarea tarea = tareasOrdenadas[i];

            DateTime fechaLimite;
            if (!sucesoras[tarea].Any())
            {
                fechaLimite = proyecto.FechaFinMasTemprana;
            }
            else
            {
                List<DateTime> posiblesLimites = new List<DateTime>();

                foreach (Tarea sucesora in sucesoras[tarea])
                {
                    foreach (Dependencia dependencia in sucesora.Dependencias.Where(d => d.Tarea == tarea))
                    {
                        if (dependencia.Tipo == "FS")
                            posiblesLimites.Add(sucesora.FechaInicioMasTemprana);
                        else if (dependencia.Tipo == "SS")
                            posiblesLimites.Add(sucesora.FechaInicioMasTemprana);
                    }
                }

                fechaLimite = posiblesLimites.Min();
            }

            tarea.Holgura = (int)(fechaLimite - tarea.FechaFinMasTemprana).TotalDays;
        }
    }

    private static List<Tarea> ordenarTopologicamente(List<Tarea> tareas)
    {
        List<Tarea> tareasOrdenadas = new List<Tarea>();
        
        Dictionary<Tarea,int> gradoEntrada = new Dictionary<Tarea, int>();
        Dictionary<Tarea, List<Tarea>> sucesoras = new Dictionary<Tarea, List<Tarea>>();
        
        foreach (Tarea tarea in tareas)
        {
            gradoEntrada[tarea] = 0;
            sucesoras[tarea] = new List<Tarea>();
        }
        
        foreach (Tarea tarea in tareas)
        {
            foreach (Dependencia dependencia in tarea.Dependencias)
            {
                Tarea anterior = dependencia.Tarea;
                gradoEntrada[tarea]++;
                sucesoras[anterior].Add(tarea);
            }
        }
        
        Queue<Tarea> cola = new Queue<Tarea>(tareas.Where(t => gradoEntrada[t] == 0));
        while (cola.Count > 0)
        {
            Tarea tareaActual = cola.Dequeue();
            tareasOrdenadas.Add(tareaActual); // ordenar tambien por menos fecha de inicio???

            foreach (Tarea siguiente in sucesoras[tareaActual])
            {
                gradoEntrada[siguiente]--;
                if (gradoEntrada[siguiente] == 0)
                {
                    cola.Enqueue(siguiente);
                }
            }
        }
        
        // Validación: si hay ciclo, no se procesaron todas
        if (tareasOrdenadas.Count != tareas.Count)
        {
            throw new ExcepcionDominio("El grafo de tareas tiene dependencias cíclicas.");
        }
        
        return tareasOrdenadas;
    }
    
    public static Dictionary<Tarea, List<Tarea>> MapearSucesoras(List<Tarea> tareas)
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
    
}