using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Inmueble
    {
        [Key]
        [Column("id_inmueble")]
        public int InmuebleId { get; set; }

        [Required]
        [StringLength(200)]
        [Column("direccion")]
        public string Direccion { get; set; }

        [Required]
        [Column("uso")]
        public string Uso { get; set; } // "comercial" o "residencial"

        [Required]
        [Column("ambientes")]
        public int Ambientes { get; set; }

        [Required]
        [StringLength(150)]
        [Column("coordenadas")]
        public string Coordenadas { get; set; }

        [Required]
        [Column("precio_base")]
        public decimal PrecioBase { get; set; }

        [Required]
        [Column("estado")]
        public byte Estado { get; set; }

        [Required]
        [ForeignKey("Propietario")]
        [Column("id_propietario")]
        public int PropietarioId { get; set; }
        public virtual Propietario Propietario { get; set; }

        [Required]
        [ForeignKey("Tipo")]
        [Column("id_tipo")]
        public int TipoId { get; set; }
        public virtual Tipo Tipo { get; set; }
    }
}
