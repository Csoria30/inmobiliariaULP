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
        



        [StringLength(45)]
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; } // "administrador" o "empleado"

        [StringLength(45)]
        public string? Avatar { get; set; }
    }
}