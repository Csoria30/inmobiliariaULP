namespace inmobiliariaULP.Models;

public class Propietario : Persona
{
    public int PropietarioId { get; set; }
    // Constructor por defecto
    public Propietario() : base() { }

    // Constructor con parámetros
    public Propietario(string dni, string apellido, string nombre, string telefono, string email) 
        : base()
    {
        Dni = dni;
        Apellido = apellido;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }

}