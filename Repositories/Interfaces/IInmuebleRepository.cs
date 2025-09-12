using inmobiliariaULP.Models;
namespace inmobiliariaULP.Repositories.Interfaces;
public interface IInmuebleRepository
{
    Task<Inmueble> AddAsync(Inmueble inmueble);
}