namespace inmobiliariaULP.Models;

public class Propietario : Persona
{
    public int PropietarioId { get; set; }
    public int PersonaId { get; set; }
    public bool Estado { get; set; }
    // Constructor por defecto
    public Propietario() : base() { }

    

}