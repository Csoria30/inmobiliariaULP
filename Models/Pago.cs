using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    [Table("pagos")]
    public class Pago
    {
        [Key]
        [Column("id_pago")]
        public int IdPago { get; set; }

        [Required]
        [Column("id_contrato")]
        public int IdContrato { get; set; }

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; }

        [Required]
        [Column("numero_pago")]
        [StringLength(45)]
        public string NumeroPago { get; set; }

        [Required]
        [Column("importe")]
        public decimal Importe { get; set; }

        [Required]
        [Column("concepto")]
        [StringLength(100)]
        public string Concepto { get; set; }

        [Required]
        [Column("estadoPago")]
        public string EstadoPago { get; set; } = "aprobado";

        // Propiedades de navegación
        [ForeignKey("IdContrato")]
        public virtual Contrato? Contrato { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }
    }
}