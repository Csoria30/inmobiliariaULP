using System.ComponentModel.DataAnnotations;

namespace inmobiliariaULP.Models.ViewModels;

public class ContratoDetalleDTO : IValidatableObject
{
    // Contrato
    public int ContratoId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    [Display(Name = "Fecha de fin")]
    public DateTime FechaFin { get; set; } 

    [Required(ErrorMessage = "El monto mensual es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto mensual debe ser mayor a 0")]
    [Display(Name = "Monto mensual")]
    public decimal MontoMensual { get; set; }

    [Display(Name = "Fecha de finalización anticipada")]
    public DateTime? FechaAnticipada { get; set; }

    [Display(Name = "Multa")]
    public decimal? Multa { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el estado del contrato")]
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

    [Display(Name = "Duración (meses)")]
    public int DuracionEnMeses
    {
        get
        {
            if (FechaInicio == default || FechaFin == default)
                return 0;
                
            // Calcular días totales y dividir por 30 (promedio de días por mes)
            var totalDias = (FechaFin - FechaInicio).Days;
            return (int)Math.Round(totalDias / 30.0);
        }
    }

    // Inmueble
    [Required(ErrorMessage = "Debe seleccionar un inmueble")]
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

    [Required(ErrorMessage = "Debe seleccionar un inquilino")]
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

    //- Validacion PERsONALIZADA
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
    



        // Validar que la fecha de fin sea mayor que la fecha de inicio
        if (FechaFin <= FechaInicio)
        {
            results.Add(new ValidationResult(
                "La fecha de fin debe ser posterior a la fecha de inicio",
                new[] { nameof(FechaFin) }
            ));
        }

        // Validar que la fecha de inicio no sea anterior a hoy (PERMITE HOY)
        if (FechaInicio < DateTime.Today)
        {
            var fechaSeleccionada = FechaInicio.ToString("dd/MM/yyyy");
            var fechaActual = DateTime.Today.ToString("dd/MM/yyyy");
            results.Add(new ValidationResult(
                $"La fecha de inicio seleccionada ({fechaSeleccionada}) es anterior al día de hoy ({fechaActual}). Por favor, seleccione una fecha igual o posterior a la fecha actual.",
                new[] { nameof(FechaInicio) }
            ));
        }

        // Validar que el contrato tenga una duración mínima
        if ((FechaFin - FechaInicio).Days < 30)
        {
            results.Add(new ValidationResult(
                "El contrato debe tener una duración mínima de 30 días",
                new[] { nameof(FechaFin) }
            ));
        }

        return results;
    }
}