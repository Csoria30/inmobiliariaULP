using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Empleado
    {
        [Key]
        [Column("id_empleado")]
        public int EmpleadoId { get; set; }

        [Required]
        [ForeignKey("Persona")]
        [Column("id_persona")]
        public int PersonaId { get; set; }
        public virtual Persona Persona { get; set; }

        [Required]
        public byte Estado { get; set; }
    }
}