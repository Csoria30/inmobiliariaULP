using System.ComponentModel.DataAnnotations;

namespace inmobiliariaULP.Models.ViewModels
{
    public class InmuebleDisponibilidadDTO
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [Display(Name = "Fecha inicio contrato")]
        [DataType(DataType.Date)]
        public DateTime? FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [Display(Name = "Fecha fin contrato")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "Uso del inmueble")]
        [StringLength(50, ErrorMessage = "El uso no puede exceder 50 caracteres")]
        public string? Uso { get; set; }

        [Display(Name = "Cantidad mínima de ambientes")]
        [Range(1, 10, ErrorMessage = "Los ambientes deben estar entre 1 y 10")]
        public int? Ambientes { get; set; }

        [Display(Name = "Precio mínimo")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio mínimo debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public decimal? PrecioMin { get; set; }

        [Display(Name = "Precio máximo")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio máximo debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public decimal? PrecioMax { get; set; }

     
        public int InmuebleId { get; set; }
        
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;
        
        [Display(Name = "Tipo de Inmueble")]
        public string TipoInmueble { get; set; } = string.Empty;
        
        [Display(Name = "Uso del Inmueble")]
        public string UsoInmueble { get; set; } = string.Empty;
        
        [Display(Name = "Precio Base")]
        [DataType(DataType.Currency)]
        public decimal PrecioBase { get; set; }
        
        [Display(Name = "Estado de Disponibilidad")]
        public string EstadoDisponibilidad { get; set; } = string.Empty;
        
        [Display(Name = "Propietario")]
        public string PropietarioNombre { get; set; } = string.Empty;
        
        public int PropietarioId { get; set; }

     
        [Display(Name = "Email del Propietario")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? PropietarioEmail { get; set; }
        
        [Display(Name = "Teléfono del Propietario")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? PropietarioTelefono { get; set; }

        // Resultados a listar en la vista
        public List<InmuebleDisponibilidadDTO>? Resultados { get; set; }
    }
}