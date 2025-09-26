using System.ComponentModel.DataAnnotations;

namespace inmobiliariaULP.Models.ViewModels;

public class ContratoDetalleDTO
{
    // Contrato
    public int ContratoId { get; set; }
    public int inquilinoId { get; set; }

    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    [Display(Name = "Fecha de fin")] 
    public DateTime FechaFin { get; set; } = DateTime.Today.AddMonths(1);   

    [Display(Name = "Monto mensual")]
    public decimal MontoMensual { get; set; }

    [Display(Name = "Fecha de finalización anticipada")]
    public DateTime? FechaAnticipada { get; set; }

    [Display(Name = "Multa")]
    public decimal? Multa { get; set; }

    [Display(Name = "Estado del contrato")]
    public string EstadoContrato { get; set; }

    [Display(Name = "Cantidad de pagos realizados")]
    public int PagosRealizados { get; set; }

    [Display(Name = "Días restantes")]
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
    
    [Display(Name = "Dirección")]
    public string Direccion { get; set; }

    [Display(Name = "Tipo de inmueble")]
    public string TipoInmueble { get; set; }

    [Display(Name = "Uso de inmueble")]
    public string UsoInmueble { get; set; }

    [Display(Name = "Cantidad de ambientes")]
    public int Ambientes { get; set; }

    [Display(Name = "Coordenadas")]
    public string Coordenadas { get; set; }

    [Display(Name = "Estado del inmueble")]
    public bool EstadoInmueble { get; set; }

    //- Propietario
    public int PropietarioId { get; set; }
    public int PropietarioIdPersona { get; set; }

    [Display(Name = "Nombre del propietario")]
    public string NombrePropietario { get; set; }

    [Display(Name = "Email de contacto")]
    public string EmailPropietario { get; set; }

    [Display(Name = "Teléfono de contacto")]
    public string TelefonoPropietario { get; set; }

    //- Inquilino
    public int InquilinoId { get; set; }
    public int InquilinoIdPersona { get; set; }

    [Display(Name = "Nombre del inquilino")]
    public string NombreInquilino { get; set; }

    [Display(Name = "Email de contacto")]
    public string EmailInquilino { get; set; }

    [Display(Name = "Teléfono de contacto")]
    public string TelefonoInquilino { get; set; }

    // Usuario que inicia el contrato
    public int UsuarioId { get; set; }

    [Display(Name = "Empleado inicio de contrato")]
    public string NombreEmpleado { get; set; }

    [Display(Name = "Email de contacto")]
    public string EmailUsuario { get; set; }
    public string RolUsuario { get; set; }

    // Usuario que finaliza el contrato (puede ser null)
    public int? UsuarioIdFin { get; set; }

    [Display(Name = "Empleado fin de contrato")]
    public string? NombreEmpleadoFin { get; set; }

    [Display(Name = "Email de contacto")]
    public string? EmailUsuarioFin { get; set; }
    public string? RolUsuarioFin { get; set; }
}