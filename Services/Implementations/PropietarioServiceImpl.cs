using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class PropietarioServiceImpl : IPropietarioService
{

    public async Task<int> EliminarAsync(int propietarioId)
    {
        try
        {
            var propietarioRepository = FactoryRepository.CreatePropietarioRepository();
            return await propietarioRepository.DeleteAsync(propietarioId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el propietario: " + ex.Message);
        }
    }

    public async Task<int> NuevoAsync(int personaId)
    {
        try
        {
            var propietarioRepository = FactoryRepository.CreatePropietarioRepository();
            return await propietarioRepository.AddAsync(personaId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el nuevo propietario: " + ex.Message);
        }
    }

    public Task<Propietario> ObtenerIdAsync(int propietarioId)
    {
        try
        {
            var propietarioRepository = FactoryRepository.CreatePropietarioRepository();
            return propietarioRepository.GetByIdAsync(propietarioId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el propietario por ID: " + ex.Message);
        }
    }

    public Task<IEnumerable<Propietario>> ObtenerTodosAsync()
    {
        try
        {
            var propietarioRepository = FactoryRepository.CreatePropietarioRepository();
            return propietarioRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener todos los propietarios: " + ex.Message);
        }
    }

    public async Task<int> ActualizarAsync(int personaId, Boolean estado)
    {
        try
        {
            var propietarioRepository = FactoryRepository.CreatePropietarioRepository();
            return await propietarioRepository.UpdateAsync(personaId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el propietario: " + ex.Message);
        }
    }
}
