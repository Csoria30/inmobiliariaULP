using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
namespace inmobiliariaULP.Services.Interfaces;

public interface IInmuebleService
{
    Task<(bool exito, string mensaje, string tipo)> CrearAsync(Inmueble inmueble);
    Task<(IEnumerable<Inmueble> Inmuebles, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null);

    Task<(Inmueble inmueble, string mensaje, string tipo)> ObtenerIdAsync(int inmuebleId);
    Task<(bool exito, string mensaje, string tipo)> EditarAsync(Inmueble inmueble);
    Task<(bool exito, string mensaje, string tipo)> CambiarEstadoAsync(int inmuebleId);
    Task<IEnumerable<InmueblePropietarioDTO>> ListarActivosAsync(string term);
}