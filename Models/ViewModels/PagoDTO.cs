using System.ComponentModel.DataAnnotations;

namespace inmobiliariaULP.Models.ViewModels
{
    public class PagoDTO
    {
        public int IdPago { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un contrato")]
        [Display(Name = "Contrato")]
        public int IdContrato { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "La fecha de pago es obligatoria")]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "El número de pago es obligatorio")]
        [Display(Name = "Número de pago")]
        [StringLength(45, ErrorMessage = "El número de pago no puede exceder los 45 caracteres")]
        public string NumeroPago { get; set; }

        [Required(ErrorMessage = "El importe es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0")]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(100, ErrorMessage = "El concepto no puede exceder los 100 caracteres")]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el estado del pago")]
        [Display(Name = "Estado del pago")]
        public string EstadoPago { get; set; } = "aprobado";

        // Información adicional para la vista
        [Display(Name = "Dirección del inmueble")]
        public string? DireccionInmueble { get; set; }

        [Display(Name = "Inquilino")]
        public string? NombreInquilino { get; set; }

        [Display(Name = "Monto mensual del contrato")]
        public decimal? MontoMensualContrato { get; set; }

        [Display(Name = "Usuario")]
        public string? NombreUsuario { get; set; }

        [Display(Name = "Email del usuario")]
        public string? EmailUsuario { get; set; }
    }
}