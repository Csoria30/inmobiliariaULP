using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class TipoServiceImpl : ITipoService
{
    private TipoRepositoryImpl GetTipoRepository()
    {
        return FactoryRepository.CreateTipoRepository();
    }

    public async Task<IEnumerable<Tipo>> ObtenerTodosAsync()
    {
        try
        {
            var tipoRepository = GetTipoRepository();
            return await tipoRepository.GetAllAsync();
            
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los tipos", ex);
        }
    }
}
