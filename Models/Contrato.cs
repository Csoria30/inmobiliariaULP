using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Contrato
    {
        [Key]
        [Column("id_contrato")]
        public int ContratoId { get; set; }

        [Required]
        [ForeignKey("Inmueble")]
        [Column("id_inmueble")]
        public int InmuebleId { get; set; }
        public virtual Inmueble Inmueble { get; set; }

        [Required]
        [ForeignKey("Inquilino")]
        [Column("id_inquilino")]
        public int InquilinoId { get; set; }
        public virtual Inquilino Inquilino { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        [Column("id_usuario")]
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public decimal MontoMensual { get; set; }

        public DateTime? FechaFinalizacionAnticipada { get; set; }

        public decimal? Multa { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; } // "vigente", "finalizado", "rescindido"
    }
}