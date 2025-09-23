using inmobiliariaULP.Models;

namespace inmobiliariaULP.Models.ViewModels;

public class CambiarPasswordDTO
{
    public string PasswordActual { get; set; }
    public string PasswordNueva { get; set; }
    public string PasswordConfirmar { get; set; }   
}
