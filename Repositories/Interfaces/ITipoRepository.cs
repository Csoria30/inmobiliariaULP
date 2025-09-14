using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface ITipoRepository
{
    Task<IEnumerable<Tipo>> GetAllAsync();
}