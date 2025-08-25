using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface IInquilinoService
{
    Task<int> NuevoAsync(Inquilino inquilino);
    Task<IEnumerable<Inquilino>> ObtenerTodosAsync();
    Task<Inquilino> ObtenerIdAsync(int inquilinoId);
    Task<int> EliminarAsync(int inquilinoId);
}