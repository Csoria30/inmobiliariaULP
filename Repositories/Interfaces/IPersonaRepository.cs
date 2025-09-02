using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;
public interface IPersonaRepository
{
    Task<int> AddAsync(Persona persona);
    Task<IEnumerable<Persona>> GetAllAsync();
    Task<Persona> GetByIdAsync(int id);
    Task<int> UpdateAsync(Persona persona);
    Task<int> DeleteAsync(int personaId, bool estado);
}