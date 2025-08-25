namespace inmobiliariaULP.Models;

public class Inquilino : Persona
{
    public int InquilinoId { get; set; }

    // Constructor por defecto
    public Inquilino() : base() { }

    // Constructor con parámetros
    public Inquilino(string dni, string apellido, string nombre, string telefono, string email) 
        : base()
    {
        Dni = dni;
        Apellido = apellido;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }
    

}