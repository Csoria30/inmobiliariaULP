using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models
{
    public class Tipo
    {
        [Key]
        [Column("id_tipo")]
        public int TipoId { get; set; }

        [Required]
        [StringLength(45)]
        [Column("descripcion")]
        public string Descripcion { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}