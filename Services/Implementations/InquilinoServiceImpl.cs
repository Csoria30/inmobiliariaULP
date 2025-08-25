using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class InquilinoServiceImpl : IInquilinoService
{

    public Task<int> EliminarAsync(int inquilinoId)
    {
        try
        {
            var inquilinoRepository = FactoryRepository.CreateInquilinoRepository();
            return inquilinoRepository.DeleteAsync(inquilinoId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el inquilino", ex);
        }
    }

    public Task<int> NuevoAsync(Inquilino inquilino)
    {
        try
        {
            var inquilinoRepository = FactoryRepository.CreateInquilinoRepository();
            return inquilinoRepository.AddAsync(inquilino.InquilinoId);
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

    public Task<IEnumerable<Inquilino>> ObtenerTodosAsync()
    {
        try
        { 
            var inquilinoRepository = FactoryRepository.CreateInquilinoRepository();
            return inquilinoRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener todos los inquilinos", ex);
        }
    }

}
