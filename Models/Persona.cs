namespace inmobiliariaULP.Models;

public abstract class Persona
{
    public int idPersona { get; set; }
    public int Dni { get; set; }
    public string? Apellido { get; set; }
    public string? Nombre { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    //Metodo ToString
    public override string ToString()
    {
        return $"{Apellido} + {Nombre}";
    }
}


