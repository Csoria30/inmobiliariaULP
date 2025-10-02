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


    //- Dispoinibles para contrato
    Task<bool> EstaDisponibleEnFechasAsync(int inmuebleId, DateTime fechaInicio, DateTime fechaFin);
    Task<string> ObtenerEstadoDisponibilidadAsync(int inmuebleId, DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<InmuebleDisponibilidadDTO>> BuscarDisponiblesAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        string? uso = null,
        string? ambientes = null,
        string? precioMin = null,
        string? precioMax = null);
        
}