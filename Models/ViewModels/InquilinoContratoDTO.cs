using System.ComponentModel.DataAnnotations;

namespace inmobiliariaULP.Models.ViewModels;

public class InquilinoContratoDTO
{
    public int InquilinoId { get; set; }
    public int InquilinoIdPersona { get; set; }
    public string Dni { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }

    public string NombreInquilino { get; set; }
    public string EmailInquilino { get; set; }
    public string TelefonoInquilino { get; set; }    
}