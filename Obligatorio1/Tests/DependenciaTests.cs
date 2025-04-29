namespace Tests;
using Dominio;
using Dominio.Excepciones;
using Dominio.Dummies;

[TestClass]

public class DependenciaTests
{
    [TestMethod]
    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        string tipo = "tipo";
        Tarea tarea = new Tarea("titulo", "descripcion", 6);
        
        Dependencia dependencia = new Dependencia(tipo, tarea);
        
        Assert.AreEqual(tipo, dependencia.Tipo);
        Assert.AreEqual(tarea, dependencia.Tarea);

    }
    
}