using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Services.Implementations;

public class PropietarioServiceImpl : IPropietarioService
{

    private readonly IPropietarioRepository _propietarioRepository;

    public PropietarioServiceImpl(IPropietarioRepository propietarioRepository)
    {
        _propietarioRepository = propietarioRepository;
    }


    public async Task<int> EliminarAsync(int propietarioId)
    {
        try
        {
            return await _propietarioRepository.DeleteAsync(propietarioId);
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
            return await _propietarioRepository.AddAsync(personaId);
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
            return _propietarioRepository.GetByIdAsync(propietarioId);
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
            return _propietarioRepository.GetAllAsync(page, pageSize, search);
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
            return _propietarioRepository.ListActiveAsync(term);
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
            return await _propietarioRepository.UpdateAsync(personaId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el propietario: " + ex.Message);
        }
    }
}
