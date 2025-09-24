using inmobiliariaULP.Models;

namespace inmobiliariaULP.Models.ViewModels;

public class DatosPersonalesDTO
{
    public string Apellido { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string? Avatar { get; set; }
    public IFormFile? AvatarFile { get; set; }
}
