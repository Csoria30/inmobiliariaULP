using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Services.Implementations;

public class InquilinoServiceImpl : IInquilinoService
{

    private readonly IInquilinoRepository _inquilinoRepository;

    public InquilinoServiceImpl(IInquilinoRepository inquilinoRepository)
    {
        _inquilinoRepository = inquilinoRepository;
    }


    public Task<int> EliminarAsync(int inquilinoId)
    {
        try
        {
            return _inquilinoRepository.DeleteAsync(inquilinoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el inquilino", ex);
        }
    }

    public Task<int> NuevoAsync(int personaId)
    {
        try
        {
            return _inquilinoRepository.AddAsync(personaId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el nuevo inquilino", ex);
        }
    }

    public Task<Inquilino> ObtenerIdAsync(int inquilinoId)
    {
        try
        {
            return _inquilinoRepository.GetByIdAsync(inquilinoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el inquilino por ID", ex);
        }
    }

    public Task<(IEnumerable<Inquilino> Inquilinos, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            return _inquilinoRepository.GetAllAsync(page, pageSize, search);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener todos los inquilinos", ex);
        }
    }

    public async Task<int> ActualizarAsync(int personaId, Boolean estado)
    {
        try
        {
            return await _inquilinoRepository.UpdateAsync(personaId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el inquilino", ex);
        }
    }
    

}
