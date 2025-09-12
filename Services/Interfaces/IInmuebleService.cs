using inmobiliariaULP.Models;
namespace inmobiliariaULP.Services.Interfaces;
public interface IInmuebleService
{
    Task<Inmueble> NuevoAsync(Inmueble inmueble);
    Task<(bool exito, string mensaje, string tipo)> CrearAsync(Inmueble inmueble);
}