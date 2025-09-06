using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class InquilinoServiceImpl : IInquilinoService
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

    public Task<int> EliminarAsync(int inquilinoId)
    {
        try
        {
            var inquilinoRepository = GetInquilinoRepository();
            return inquilinoRepository.DeleteAsync(inquilinoId);
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
            var inquilinoRepository = GetInquilinoRepository();
            return inquilinoRepository.AddAsync(personaId);
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
            var inquilinoRepository = FactoryRepository.CreateInquilinoRepository();
            return inquilinoRepository.GetByIdAsync(inquilinoId);
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
            var inquilinoRepository = GetInquilinoRepository();
            return inquilinoRepository.GetAllAsync(page, pageSize, search);
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
            var inquilinoRepository = GetInquilinoRepository();
            return await inquilinoRepository.UpdateAsync(personaId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el inquilino", ex);
        }
    }
    

}
