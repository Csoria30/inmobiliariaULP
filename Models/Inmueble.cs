using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mysqlx;

namespace inmobiliariaULP.Models
{
    public class Inmueble
    {
        [Key]
        [Column("id_inmueble")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "La Direccion es obligatoria.")]
        [StringLength(200)]
        [Column("direccion")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el uso del inmueble.")]
        [Column("uso")]
        public string Uso { get; set; } // "comercial" o "residencial"

        [Required(ErrorMessage = "La cantidad de ambientes es obligatoria.")]
        [Column("ambientes")]
        public int? Ambientes { get; set; }

        [Required(ErrorMessage = "Las coordenadas son obligatorias.")]
        [StringLength(150)]
        [Column("coordenadas")]
        public string Coordenadas { get; set; }

        [Required(ErrorMessage = "El precio base es obligatorio.")]
        [Column("precio_base")]
        public decimal? PrecioBase { get; set; }

        
        [Column("estado")]
        public byte? Estado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        [ForeignKey("Propietario")]
        [Column("id_propietario")]
        public int? PropietarioId { get; set; }
        

        [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
        [ForeignKey("Tipo")]
        [Column("id_tipo")]
        public int? TipoId { get; set; }
        
    }
}
