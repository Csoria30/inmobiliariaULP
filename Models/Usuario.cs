using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int UsuarioId { get; set; }

        [Required]
        [ForeignKey("Empleado")]
        [Column("id_empleado")]
        public int EmpleadoId { get; set; }
        public virtual Empleado Empleado { get; set; }

        [Required, StringLength(45)]
        public string Password { get; set; }

        [Required, StringLength(20)]
        public string Rol { get; set; } // "administrador" o "empleado"

        [StringLength(45)]
        public string? Avatar { get; set; }
    }
}