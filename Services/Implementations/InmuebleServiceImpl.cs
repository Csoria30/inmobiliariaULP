using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class InmuebleServiceImpl : IInmuebleService
{


    // Getters Fabrica
    private InmuebleRepositoryImpl GetInmuebleRepository()
    {
        return FactoryRepository.CreateInmuebleRepository();
    }

    public async Task<(bool exito, string mensaje, string tipo)> CrearAsync(Inmueble inmueble)
    {
        try
        {
            // Validación de negocio
            if (inmueble.TipoId == 0)
                return (false, "Debe seleccionar un tipo de inmueble.", "warning");

            if (inmueble.PropietarioId == null || inmueble.PropietarioId == 0)
                return (false, "Debe seleccionar un propietario.", "warning");

            var inmuebleRepository = GetInmuebleRepository();
            await inmuebleRepository.AddAsync(inmueble);

            return (true, "Inmueble creado con éxito.", "success");
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el inmueble", ex);
        }
    }

    public Task<(IEnumerable<Inmueble> Inmuebles, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            var inmuebleRepository = GetInmuebleRepository();
            return inmuebleRepository.GetAllAsync(page, pageSize, search);
        }catch (Exception ex)
        {
            throw new Exception("Error al obtener los inmuebles", ex);
        }
    }
}