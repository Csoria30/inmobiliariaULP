using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IPersonaRepository
{
    Task<int> AddAsync(Persona persona);
    Task<(IEnumerable<Persona> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Persona> GetByIdAsync(int id);
    Task<PersonaUsuarioDTO> GetDetalleByIdAsync(int id);
    Task<int> UpdateAsync(Persona persona);
    Task<int> DeleteAsync(int personaId, bool estado);
    Task<List<string>> GetTiposAsync(int id);
}