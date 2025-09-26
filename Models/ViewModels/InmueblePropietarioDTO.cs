namespace inmobiliariaULP.Models.ViewModels;

public class InmueblePropietarioDTO
{
   // Inmueble
    public int InmuebleId { get; set; }
    public string Direccion { get; set; }
    public string UsoInmueble { get; set; }
    public int Ambientes { get; set; }
    public string Coordenadas { get; set; }
    public decimal PrecioBase { get; set; }
    public bool EstadoInmueble { get; set; }
    public int TipoId { get; set; }
    public string TipoInmueble { get; set; }

    // Propietario
    public int PropietarioId { get; set; }
    public string NombrePropietario { get; set; }
    public string EmailPropietario { get; set; }
    public string TelefonoPropietario { get; set; }
}