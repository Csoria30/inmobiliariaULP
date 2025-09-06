using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IInquilinoService
{
    Task<int> NuevoAsync(int personaId);
    Task<(IEnumerable<Inquilino> Inquilinos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);
    Task<Inquilino> ObtenerIdAsync(int inquilinoId);
    Task<int> EliminarAsync(int inquilinoId);
    Task<int> ActualizarAsync(int personaId, Boolean estado); 
}