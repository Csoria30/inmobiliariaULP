using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaULP.Models;

public class Propietario : Persona
{
    [Key]
    [Column("id_propietario")]
    public int PropietarioId { get; set; }

    [Required]
    [ForeignKey("Persona")]
    [Column("id_persona")]
    public new int PersonaId { get; set; }

    [Required]
    public new bool Estado { get; set; }


    // Constructor por defecto
    public Propietario() : base() { }



}