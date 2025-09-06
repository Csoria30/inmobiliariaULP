using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IPropietarioRepository
{
    Task<int> AddAsync(int personaId);
    Task<(IEnumerable<Propietario> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Propietario> GetByIdAsync(int propietarioId);
    Task<int> DeleteAsync(int propietarioId);
    Task<int> UpdateAsync(int propietarioId, bool estado);
}