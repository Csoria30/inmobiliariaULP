using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class PropietarioServiceImpl : IPropietarioService
{

    private PersonaRepositoryImpl GetPersonaRepository()
    {
        return FactoryRepository.CreatePersonaRepository();
    }

    private InquilinoRepositoryImpl GetInquilinoRepository()
    {
        return FactoryRepository.CreateInquilinoRepository();
    }

    private PropietarioRepositoryImpl GetPropietarioRepository()
    {
        return FactoryRepository.CreatePropietarioRepository();
    }

    public async Task<int> EliminarAsync(int propietarioId)
    {
        try
        {
            var propietarioRepository = GetPropietarioRepository();
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
            var propietarioRepository = GetPropietarioRepository();
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
            var propietarioRepository = GetPropietarioRepository();
            return propietarioRepository.GetByIdAsync(propietarioId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el propietario por ID: " + ex.Message);
        }
    }

    public Task<(IEnumerable<Propietario> Propietarios, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            var propietarioRepository = GetPropietarioRepository();
            return propietarioRepository.GetAllAsync(page, pageSize, search);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener todos los propietarios: " + ex.Message);
        }
    }

    public Task<IEnumerable<Propietario>> ListarActivosAsync(string term)
    {
        try
        {
            var propietarioRepository = GetPropietarioRepository();
            return propietarioRepository.ListActiveAsync(term);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al buscar propietarios activos: " + ex.Message);
        }
    }

    public async Task<int> ActualizarAsync(int personaId, Boolean estado)
    {
        try
        {
            var propietarioRepository = GetPropietarioRepository();
            return await propietarioRepository.UpdateAsync(personaId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el propietario: " + ex.Message);
        }
    }
}
