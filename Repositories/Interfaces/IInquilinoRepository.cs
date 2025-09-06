using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IInquilinoRepository
{
    Task<int> AddAsync(int personaId);
    Task<(IEnumerable<Inquilino> Personas, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Inquilino> GetByIdAsync(int inquilinoId);
    Task<int> DeleteAsync(int inquilinoId);
    Task<int> UpdateAsync(int inquilinoId, bool estado);
}