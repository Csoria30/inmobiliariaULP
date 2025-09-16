using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IInmuebleRepository
{
    Task<Inmueble> AddAsync(Inmueble inmueble);
    Task<(IEnumerable<Inmueble> Inmuebles, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
}