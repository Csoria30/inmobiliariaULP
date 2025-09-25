using inmobiliariaULP.Models;

namespace inmobiliariaULP.Models.ViewModels;

public class ContratoListadoDTO
{
    public int ContratoId { get; set; }
    public string Direccion { get; set; }
    public string TipoInmueble { get; set; }
    public string NombrePropietario { get; set; }
    public string NombreInquilino { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal MontoMensual { get; set; }
    public string EstadoContrato { get; set; }
    public int PagosRealizados { get; set; }
}