using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class PersonaServiceImpl : IPersonaService
{
    public async Task<int> ActualizarAsync(Persona persona)
    {
        throw new NotImplementedException();
    }

    public async Task<int> EliminarAsync(int personaId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> NuevoAsync(Persona persona)
    {
        throw new NotImplementedException();
    }

    public async Task<Persona> ObtenerIdAsync(int personaId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Persona>> ObtenerTodosAsync()
    {
        try
        {
            var personaRepository = FactoryRepository.CreatePersonaRepository();
            return await personaRepository.GetAllAsync();   
        }catch (Exception ex)
        {
            throw new Exception("Error al obtener las personas", ex);
        }
    }
}
