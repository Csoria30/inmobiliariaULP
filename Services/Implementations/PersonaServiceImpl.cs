using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Implementations;

namespace inmobiliariaULP.Services.Implementations;

public class PersonaServiceImpl : IPersonaService
{
    public async Task<int> ActualizarAsync(Persona persona)
    {
        try
        {
            var personaRepository = FactoryRepository.CreatePersonaRepository();
            return await personaRepository.UpdateAsync(persona);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar la persona", ex);
        }
    }

    public async Task<int> EliminarAsync(int personaId, bool estado)
    {
        try
        {
            var personaRepository = FactoryRepository.CreatePersonaRepository();
            return await personaRepository.DeleteAsync(personaId, estado);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar la persona", ex);
        }
    }

    public async Task<(bool exito, string mensaje, string tipo)> CambiarEstadoAsync(int personaId)
    {
        var personaActual = await ObtenerIdAsync(personaId);
        if (personaActual == null)
            return (false, "La persona no existe.", "danger");

        bool nuevoEstado = !personaActual.Estado;
        await EliminarAsync(personaId, nuevoEstado);

        if (nuevoEstado)
            return (true, "Persona habilitada correctamente", "success");
        else
            return (true, "Persona deshabilitada correctamente", "danger");
    }

    public async Task<int> NuevoAsync(Persona persona)
    {
        try
        {
            var personaRepository = FactoryRepository.CreatePersonaRepository();
            return await personaRepository.AddAsync(persona);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear la nueva persona", ex);
        }
    }

    public async Task<Persona> ObtenerIdAsync(int personaId)
    {
        try
        {
            var personaRepository = FactoryRepository.CreatePersonaRepository();
            return await personaRepository.GetByIdAsync(personaId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener la persona por ID", ex);
        }
    }

    public async Task<IEnumerable<Persona>> ObtenerTodosAsync()
    {
        try
        {
            var personaRepository = FactoryRepository.CreatePersonaRepository();
            var personas = await personaRepository.GetAllAsync(); // Obtener las personas

            return personas; // Retornamos la lista de personas 
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener las personas", ex);
        }
    }

    public async Task<(Persona persona, string mensaje, string tipo)> ObtenerDetalleAsync(int id)
    {
        try
        {
            var persona = await ObtenerIdAsync(id);
            if (persona == null)
                return (null, "La persona no existe.", "danger");

            return (persona, null, null);
        }
        catch (Exception ex)
        {
            return (null, "Error al cargar las personas: " + ex.Message, "danger");
        }
    }

}