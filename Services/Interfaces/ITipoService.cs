using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;

public interface ITipoService
{
    Task<IEnumerable<Tipo>> ObtenerTodosAsync();
}