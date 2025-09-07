using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Pago
    {
        [Key]
        [Column("id_pago")]
        public int PagoId { get; set; }

        [Required]
        [ForeignKey("Contrato")]
        [Column("id_contrato")]
        public int ContratoId { get; set; }
        public virtual Contrato Contrato { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        [Column("id_usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required, StringLength(45)]
        public string NumeroPago { get; set; }

        [Required]
        public decimal Importe { get; set; }

        [Required, StringLength(100)]
        public string Concepto { get; set; }

        [Required, StringLength(20)]
        public string EstadoPago { get; set; } // "aprobado", "anulado"
    }
}