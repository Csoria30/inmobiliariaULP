using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IPropietarioService
{
    Task<int> NuevoAsync(int personaId);
    Task<(IEnumerable<Propietario> Propietarios, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);
    Task<Propietario> ObtenerIdAsync(int propietarioId);
    Task<int> EliminarAsync(int propietarioId);
    Task<int> ActualizarAsync(int personaId, Boolean estado);
}