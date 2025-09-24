using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IContratosRepository
{
    Task<int> AddAsync(Contrato contrato);
    Task<(IEnumerable<Contrato> Contratos, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Contrato> GetByIdAsync(int contratoId);
    Task<int> DeleteAsync(int contratoId);
    Task<int> UpdateAsync(int contratoId);    
}