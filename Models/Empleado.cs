using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Empleado : Persona
    {
        [Key]
        [Column("id_empleado")]
        public int EmpleadoId { get; set; }

        [Required]
        [ForeignKey("Persona")]
        [Column("id_persona")]
        public int PersonaId { get; set; }

        [Required]
        public bool Estado { get; set; }

        public Empleado() : base() { }
    }
}