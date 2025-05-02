using Dominio.Excepciones;
using Servicios.Utilidades;

namespace Tests;

[TestClass]
public class UtilidadesContrasenaTests
{
    [TestMethod]
    public void ContrasenaEncriptadaEsDistintaALaOriginal()
    {
        string unaContrasena = "Contrase#a3";
        string contrasenaEncriptada = UtilidadesContrasena.EncriptarContrasena(unaContrasena);
        Assert.AreNotEqual(unaContrasena, contrasenaEncriptada);
    }

    [TestMethod]
    public void SePuedeVerificarContrasenaEncriptada()
    {
        string unaContrasena = "Contrase#a3";
        string contrasenaEncriptada = UtilidadesContrasena.EncriptarContrasena(unaContrasena);
        Assert.IsTrue(BCrypt.Net.BCrypt.Verify(unaContrasena, contrasenaEncriptada));
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void IngresoDeContrasenaMuyCorta()
    {
        UtilidadesContrasena.ValidarFormatoContrasena("P3e.");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void IngresoDeContrasenaSinMayusculas()
    {
        UtilidadesContrasena.ValidarFormatoContrasena("minuscula1@");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void IngresoDeContrasenaSinMinusculas()
    {
        UtilidadesContrasena.ValidarFormatoContrasena("MAYUSCULA1@");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void IngresoDeContrasenaSinNumeros()
    {
        UtilidadesContrasena.ValidarFormatoContrasena("CoNtRaSeN@");
    }

    [ExpectedException(typeof(ExcepcionServicios))]
    [TestMethod]
    public void IngresoDeContrasenaSinCaracterEspecial()
    {
        UtilidadesContrasena.ValidarFormatoContrasena("CoNtRaSeN14");
    }

    [TestMethod]
    public void SeAutogeneraUnaContrasenaValida()
    {
        string contrasenaAutogenerada = UtilidadesContrasena.AutogenerarContrasenaValida();
        UtilidadesContrasena.ValidarFormatoContrasena(contrasenaAutogenerada);
    }
}