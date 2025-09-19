using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Services.Implementations;

public class TipoServiceImpl : ITipoService
{
    private readonly ITipoRepository _tipoRepository;

    public TipoServiceImpl(ITipoRepository tipoRepository)
    {
        _tipoRepository = tipoRepository;
    }


    public async Task<IEnumerable<Tipo>> ObtenerTodosAsync()
    {
        try
        {
            return await _tipoRepository.GetAllAsync();
            
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los tipos", ex);
        }
    }
}
