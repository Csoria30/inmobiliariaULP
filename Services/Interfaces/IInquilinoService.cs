using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IInquilinoService
{
    Task<int> NuevoAsync(int personaId);
    Task<IEnumerable<Inquilino>> ObtenerTodosAsync();
    Task<Inquilino> ObtenerIdAsync(int inquilinoId);
    Task<int> EliminarAsync(int inquilinoId);
    Task<int> ActualizarAsync(int personaId, Boolean estado); 
}