namespace inmobiliariaULP.Models.ViewModels;

public class ContratoDetalleDTO
{
    // Contrato
    public int ContratoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal MontoMensual { get; set; }
    public DateTime? FechaAnticipada { get; set; }
    public decimal? Multa { get; set; }
    public string EstadoContrato { get; set; }
    public int PagosRealizados { get; set; }
    public int DiasRestantes
    {
        get
        {
            var dias = (FechaFin - DateTime.Today).Days;
            return dias < 0 ? 0 : dias;
        }
    }

    // Inmueble
    public int InmuebleId { get; set; }
    public string Direccion { get; set; }
    public string TipoInmueble { get; set; }
    public string UsoInmueble { get; set; }
    public int Ambientes { get; set; }
    public string Coordenadas { get; set; }
    public bool EstadoInmueble { get; set; }

    // Propietario
    public int PropietarioId { get; set; }
    public int PropietarioIdPersona { get; set; }
    public string NombrePropietario { get; set; }
    public string EmailPropietario { get; set; }

    // Inquilino
    public int InquilinoId { get; set; }
    public int InquilinoIdPersona { get; set; }
    public string NombreInquilino { get; set; }
    public string EmailInquilino { get; set; }

    // Usuario que inicia el contrato
    public int UsuarioId { get; set; }
    public string NombreEmpleado { get; set; }
    public string EmailUsuario { get; set; }
    public string RolUsuario { get; set; }

    // Usuario que finaliza el contrato (puede ser null)
    public int? UsuarioIdFin { get; set; }
    public string NombreEmpleadoFin { get; set; }
    public string EmailUsuarioFin { get; set; }
    public string RolUsuarioFin { get; set; }
}