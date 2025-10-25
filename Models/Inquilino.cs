using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace inmobiliariaULP.Models;

public class Inquilino : Persona
{
    [Key]
    [Column("id_inquilino")]
    public int InquilinoId { get; set; }

    [Required]
    [ForeignKey("Persona")]
    [Column("id_persona")]
    public new int PersonaId { get; set; }

    [Required]
    public new bool Estado { get; set; }

    // Constructor por defecto
    public Inquilino() : base() { }



}