using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Repositories.Interfaces;

public interface IInmuebleRepository
{
    Task<int> AddAsync(Inmueble inmueble);
    Task<(IEnumerable<Inmueble> Inmuebles, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    Task<Inmueble> GetByIdAsync(int inmuebleId);
    Task<int> UpdateAsync(Inmueble inmueble);
    Task<int> DeleteAsync(int inmuebleId, bool estado);
    Task<IEnumerable<InmueblePropietarioDTO>> ListActiveAsync(string term);
    Task<IEnumerable<InmuebleDisponibilidadDTO>> SearchDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin, string? uso = null, int? ambientes = null, decimal? precioMin = null,  decimal? precioMax = null);
}