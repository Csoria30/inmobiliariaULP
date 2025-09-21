using inmobiliariaULP.Models;

namespace inmobiliariaULP.Models.ViewModels;
public class UsuarioLoginDTO
{
    public int UsuarioId { get; set; }
    public int EmpleadoId { get; set; }
    public string Password { get; set; }
    public string Rol { get; set; }
    public string Avatar { get; set; }
    public bool Estado { get; set; }
    public string Email { get; set; }
    public string Apellido { get; set; }
    public string Nombre { get; set; }
}