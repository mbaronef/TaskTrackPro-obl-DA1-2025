using Controladores;
using Excepciones;
using Moq;
using Servicios.Exportacion;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorExportacionTests
{
    private ControladorExportacion _controladorExportacion;
    private Mock<IExportadorProyectos> _mockExportadorCsv;
    private Mock<IExportadorProyectos> _mockExportadorJson;
    
    [TestInitialize]
    public void Setup()
    {
        _mockExportadorCsv = new Mock<IExportadorProyectos>();
        _mockExportadorJson = new Mock<IExportadorProyectos>();
        _mockExportadorCsv.Setup(e => e.NombreFormato).Returns("csv");
        _mockExportadorJson.Setup(e => e.NombreFormato).Returns("json");

        List<IExportadorProyectos> exportadores = new List<IExportadorProyectos>
        {
            _mockExportadorCsv.Object,
            _mockExportadorJson.Object
        };

        _controladorExportacion = new ControladorExportacion(exportadores);
    }
    
    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorExportacion);
    }

    [TestMethod]
    public async Task ExportarCsv_LlamaCorrectamenteAlExportador()
    {
        _mockExportadorCsv.Setup(e => e.Exportar()).ReturnsAsync(new byte[0]);
        
        await _controladorExportacion.Exportar("csv");

        _mockExportadorCsv.Verify(e => e.Exportar(), Times.Once);
        _mockExportadorJson.Verify(e => e.Exportar(), Times.Never);
    }
    
    [TestMethod]
    public async Task ExportarJson_LlamaCorrectamenteAlExportador()
    {
        _mockExportadorCsv.Setup(e => e.Exportar()).ReturnsAsync(new byte[0]);
        
        await _controladorExportacion.Exportar("json");

        _mockExportadorCsv.Verify(e => e.Exportar(), Times.Never);
        _mockExportadorJson.Verify(e => e.Exportar(), Times.Once);
    }
    
    [ExpectedException(typeof(ExcepcionExportador))]
    [TestMethod]
    public async Task Exportar_FormatoNoExistente_LanzaExcepcion()
    {
        _mockExportadorCsv.Setup(e => e.NombreFormato).Returns("csv");
        _mockExportadorJson.Setup(e => e.NombreFormato).Returns("json");
        
        await _controladorExportacion.Exportar("xls"); 
    }
}