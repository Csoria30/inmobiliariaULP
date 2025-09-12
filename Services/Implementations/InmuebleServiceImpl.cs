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


    public async Task<Inmueble> NuevoAsync(Inmueble inmueble)
    {
        try
        {
            var inmuebleRepository = GetInmuebleRepository();
            return await inmuebleRepository.AddAsync(inmueble);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el inmueble", ex);
        }
    }

    public async Task<(bool exito, string mensaje, string tipo)> CrearAsync(Inmueble inmueble)
    {
        try
        {
         return (true, "Inmueble creado con éxito.", "success");   
        }catch (Exception ex)
        {
            throw new Exception("Error al crear el inmueble", ex);
        }
    }
}