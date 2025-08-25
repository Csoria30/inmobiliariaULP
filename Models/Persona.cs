using System.ComponentModel.DataAnnotations;
namespace inmobiliariaULP.Models;

public class Persona
{
    public int PersonaId { get; set; }
    public string Dni { get; set; }
    public string Apellido { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }




    //Metodo ToString
    public override string ToString()
    {
        return $"{Apellido} + {Nombre}";
    }
}