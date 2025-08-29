using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IPropietarioService
{
    Task<int> NuevoAsync(int personaId);
    Task<IEnumerable<Propietario>> ObtenerTodosAsync();
    Task<Propietario> ObtenerIdAsync(int propietarioId);

    Task<int> EliminarAsync(int propietarioId);
}