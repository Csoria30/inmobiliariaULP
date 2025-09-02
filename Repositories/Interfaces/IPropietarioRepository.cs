using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IPropietarioRepository
{
    Task<int> AddAsync(int personaId);
    Task<IEnumerable<Propietario>> GetAllAsync();
    Task<Propietario> GetByIdAsync(int propietarioId);
    Task<int> DeleteAsync(int propietarioId);
    Task<int> UpdateAsync(int propietarioId, bool estado);
}