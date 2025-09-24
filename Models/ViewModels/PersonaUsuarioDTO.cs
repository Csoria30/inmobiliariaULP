using inmobiliariaULP.Models;

namespace inmobiliariaULP.Models.ViewModels;

public class PersonaUsuarioDTO
{
    public int PersonaId { get; set; }
    public int EmpleadoId { get; set; }
    public string Dni { get; set; }
    public string Apellido { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public bool Estado { get; set; }
    public List<string> TipoPersona { get; set; }
    public string Password { get; set; }
    public string Rol { get; set; } // "administrador" o "empleado"
    public string? Avatar { get; set; }
    public IFormFile? AvatarFile { get; set; }
    
}
