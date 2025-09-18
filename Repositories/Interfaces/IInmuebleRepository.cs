using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IInmuebleRepository
{
    Task<Inmueble> AddAsync(Inmueble inmueble);
    Task<(IEnumerable<Inmueble> Inmuebles, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Inmueble> GetByIdAsync(int inmuebleId);
    Task<int> UpdateAsync(Inmueble inmueble);
    Task<int> DeleteAsync(int inmuebleId, bool estado);
}