using System.ComponentModel.DataAnnotations;
namespace inmobiliariaULP.Models;

public class Persona
{
    public int PersonaId { get; set; }

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe contener 7 u 8 dígitos numéricos.")]
    public string Dni { get; set; }

    [Required(ErrorMessage = "El Apellido es obligatorio.")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "El Nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El Teléfono es obligatorio.")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "El Email es obligatorio.")]
    public string Email { get; set; }




    //Metodo ToString
    public override string ToString()
    {
        return $"{Apellido} + {Nombre}";
    }
}