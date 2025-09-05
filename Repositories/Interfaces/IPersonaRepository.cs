using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;
public interface IPersonaRepository
{
    Task<int> AddAsync(Persona persona);
    Task<(IEnumerable<Persona> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Persona> GetByIdAsync(int id);
    Task<int> UpdateAsync(Persona persona);
    Task<int> DeleteAsync(int personaId, bool estado);
}