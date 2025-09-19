using System.Data;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;

namespace inmobiliariaULP.Services.Implementations;

public class PersonaServiceImpl : IPersonaService
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IInquilinoRepository _inquilinoRepository;
    private readonly IPropietarioRepository _propietarioRepository;

    public PersonaServiceImpl(
        IPersonaRepository personaRepository
        , IInquilinoRepository inquilinoRepository
        , IPropietarioRepository propietarioRepository
    )
    {
        _personaRepository = personaRepository;
        _inquilinoRepository = inquilinoRepository;
        _propietarioRepository = propietarioRepository;
    }
    


    public async Task<int> ActualizarAsync(Persona persona)
    {
        try
        {
            return await _personaRepository.UpdateAsync(persona);
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
            return await _personaRepository.DeleteAsync(personaId, estado);
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
            return await _personaRepository.AddAsync(persona);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear la nueva persona", ex);
        }
    }

    public async Task<(bool exito, string mensaje, string tipo)> CrearAsync(Persona persona)
    {
        if (persona.TipoPersona == null || persona.TipoPersona.Count == 0 || !persona.TipoPersona.Any())
            return (false, "Debe seleccionar al menos un perfil (Inquilino y/o Propietario).", "danger");

        try
        {
            // Crear persona y obtener ID
            var personaId = await NuevoAsync(persona);


            // Crear perfiles
            foreach (var tipo in persona.TipoPersona)
            {
                switch (tipo)
                {
                    case "inquilino":
                        await _inquilinoRepository.AddAsync(personaId);
                        break;
                    case "propietario":
                        await _propietarioRepository.AddAsync(personaId);
                        break;
                    default:
                        return (false, "Tipo de persona inválido.", "danger");
                }
            }

            return (true, "Persona creada correctamente.", "success");
        }
        catch (Exception ex)
        {
            return (false, "Error al crear la persona: " + ex.Message, "danger");
        }
    }

    public async Task<Persona> ObtenerIdAsync(int personaId)
    {
        try
        {
            return await _personaRepository.GetByIdAsync(personaId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener la persona por ID", ex);
        }
    }

    public async Task<(IEnumerable<Persona> Personas, int Total)> ObtenerTodosAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            return await _personaRepository.GetAllAsync(page, pageSize, search);
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

    public async Task<(bool exito, string mensaje, string tipo)> EditarAsync(Persona persona)
    {
         var personaActual = await ObtenerIdAsync(persona.PersonaId);

        if (personaActual == null)
            return (false, "La persona no existe.", "danger");

        // Verificar cambios en datos personales
        var camposPersonales = new[] { "Nombre", "Apellido", "Dni", "Telefono", "Email" };
        bool datosPersonalesCambiaron = false;

        foreach (var campo in camposPersonales)
        {
            var valorOriginal = personaActual.GetType().GetProperty(campo)?.GetValue(personaActual)?.ToString();
            var valorNuevo = persona.GetType().GetProperty(campo)?.GetValue(persona)?.ToString();

            if (valorOriginal != valorNuevo)
            {
                datosPersonalesCambiaron = true;
                break;
            }
        }

        var notificaciones = new List<string>();

        if (datosPersonalesCambiaron)
        {
            personaActual.Nombre = persona.Nombre;
            personaActual.Apellido = persona.Apellido;
            personaActual.Dni = persona.Dni;
            personaActual.Telefono = persona.Telefono;
            personaActual.Email = persona.Email;
            await ActualizarAsync(personaActual);
            notificaciones.Add("Datos personales actualizados correctamente");
        }

        //- PROPIETARIO
        var propietario = await _propietarioRepository.GetByIdAsync(personaActual.PersonaId);
        bool esPropietario = persona.TipoPersona.Contains("propietario");
        if (esPropietario)
        {
            if (propietario == null)
            {
                await _propietarioRepository.AddAsync(personaActual.PersonaId);
                notificaciones.Add("Perfil propietario asignado correctamente");
            }
            else if (!propietario.Estado)
            {
                await _propietarioRepository.UpdateAsync(propietario.PropietarioId, true);
                notificaciones.Add("Perfil propietario habilitado correctamente");
            }
        }
        else if (propietario != null && propietario.Estado)
        {
            await _propietarioRepository.UpdateAsync(propietario.PropietarioId, false);
            notificaciones.Add("Perfil propietario deshabilitado correctamente");
        }


        //- INQUILINO
        var inquilino = await _inquilinoRepository.GetByIdAsync(personaActual.PersonaId);
        bool esInquilino = persona.TipoPersona.Contains("inquilino");
        if (esInquilino)
        {
            if (inquilino == null)
            {
                await _inquilinoRepository.AddAsync(personaActual.PersonaId);
                notificaciones.Add("Perfil inquilino asignado correctamente");
            }
            else if (!inquilino.Estado)
            {
                await _inquilinoRepository.UpdateAsync(inquilino.InquilinoId, true);
                notificaciones.Add("Perfil inquilino habilitado correctamente");
            }
        }
        else if (inquilino != null && inquilino.Estado)
        {
            await _inquilinoRepository.UpdateAsync(inquilino.InquilinoId, false);
            notificaciones.Add("Perfil inquilino deshabilitado correctamente");
        }

        if (notificaciones.Count == 0)
            return (true, "No hubo cambios.", "info");

        var tipo = notificaciones.Any(n => n.Contains("deshabilitado")) ? "danger" : "success";
        return (true, string.Join(". ", notificaciones), tipo);
    }
}