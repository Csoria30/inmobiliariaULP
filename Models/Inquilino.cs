namespace inmobiliariaULP.Models;

public class Inquilino : Persona
{
    public int InquilinoId { get; set; }
    public int PersonaId { get; set; }
    public bool Estado { get; set; }  

    // Constructor por defecto
    public Inquilino() : base() { }

    

}