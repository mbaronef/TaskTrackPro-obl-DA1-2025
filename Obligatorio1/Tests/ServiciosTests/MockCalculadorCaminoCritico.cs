using Dominio;
using Interfaces.InterfacesServicios;

namespace Tests.ServiciosTests;

public class MockCalculadorCaminoCritico : ICalculadorCaminoCritico
{
    public bool FueLlamado { get; private set; } = false;

    public void CalcularCaminoCritico(Proyecto proyecto)
    {
        FueLlamado = true;
    }
}