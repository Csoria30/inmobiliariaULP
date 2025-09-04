using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IPersonaService
{
    Task<int> NuevoAsync(Persona persona);
    Task<IEnumerable<Persona>> ObtenerTodosAsync();
    Task<Persona> ObtenerIdAsync(int personaId);
    Task<int> ActualizarAsync(Persona persona);
    Task<int> EliminarAsync(int personaId, bool estado);

    Task<(bool exito, string mensaje, string tipo)> CambiarEstadoAsync(int personaId);
    Task<(Persona persona, string mensaje, string tipo)> ObtenerDetalleAsync(int id);
    Task<(bool exito, string mensaje, string tipo)> CrearAsync(Persona persona);
    Task<(bool exito, string mensaje, string tipo)> EditarAsync(Persona persona);
}