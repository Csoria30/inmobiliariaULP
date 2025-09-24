using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;

namespace inmobiliariaULP.Services.Interfaces;

public interface IPersonaService
{

    Task<(IEnumerable<Persona> Personas, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);
    Task<Persona> ObtenerIdAsync(int personaId);
    Task<(PersonaUsuarioDTO persona, string mensaje, string tipo)> ObtenerDtoIdAsync(int personaId);
    Task<int> ActualizarAsync(Persona persona);
    Task<int> EliminarAsync(int personaId, bool estado);
    Task<(bool exito, string mensaje, string tipo)> CambiarEstadoAsync(int personaId);
    Task<(Persona persona, string mensaje, string tipo)> ObtenerDetalleAsync(int id);
    Task<(bool exito, string mensaje, string tipo, int personaId)> CrearAsync(Persona persona);
    Task<(bool exito, string mensaje, string tipo)> EditarAsync(Persona persona);
    Task<bool> EsEmpleado(int personaId);
    Task<(DatosPersonalesDTO datos, bool exito)> ObtenerDatosPersonalesByEmailAsync(string email);
    Task<(DatosPersonalesDTO datos, bool exito)> ActualizarDatosPersonalesAsync(DatosPersonalesDTO datos);
    Task<bool> CambiarPasswordAsync(CambiarPasswordDTO cambiarPasswordDTO, string email);
    Task<string> ObtenerPasswordByEmpleadoIdAsync(int empleadoId);
    Task<string> ObtenerImagenPerfilByIdAsync(int empleadoId);
}