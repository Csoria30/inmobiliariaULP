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
}