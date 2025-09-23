using System.Data;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Helpers;

namespace inmobiliariaULP.Services.Implementations;

public class PersonaServiceImpl : IPersonaService
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IInquilinoRepository _inquilinoRepository;
    private readonly IPropietarioRepository _propietarioRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IConfiguration _configuration;
    private readonly IUsuarioRepository _usuarioRepository;

    public PersonaServiceImpl(
        IPersonaRepository personaRepository
        , IInquilinoRepository inquilinoRepository
        , IPropietarioRepository propietarioRepository
        , IEmpleadoRepository empleadoRepository
        , IConfiguration configuration,
        IUsuarioRepository usuarioRepository
    )
    {
        _personaRepository = personaRepository;
        _inquilinoRepository = inquilinoRepository;
        _propietarioRepository = propietarioRepository;
        _empleadoRepository = empleadoRepository;
        _configuration = configuration;
        _usuarioRepository = usuarioRepository;
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
        var personaActual = await _personaRepository.GetByIdAsync(personaId);
        if (personaActual == null)
            return (false, "La persona no existe.", "danger");

        bool nuevoEstado = !personaActual.Estado;
        await EliminarAsync(personaId, nuevoEstado);

        if (nuevoEstado)
            return (true, "Persona habilitada correctamente", "success");
        else
            return (true, "Persona deshabilitada correctamente", "danger");
    }


    public async Task<(bool exito, string mensaje, string tipo, int personaId)> CrearAsync(Persona persona)
    {
        if (persona.TipoPersona == null || persona.TipoPersona.Count == 0 || !persona.TipoPersona.Any())
            return (false, "Debe seleccionar al menos un perfil (Inquilino y/o Propietario).", "danger", 0);

        try
        {
            // Crear persona y obtener ID
            var personaId = await _personaRepository.AddAsync(persona);


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
                    case "empleado":
                        await _empleadoRepository.AddAsync(personaId);
                        break;
                    default:
                        return (false, "Tipo de persona inválido.", "danger", 0);
                }
            }

            return (true, "Persona creada correctamente.", "success", personaId);
        }
        catch (Exception ex)
        {
            return (false, "Error al crear la persona: " + ex.Message, "danger", 0);
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

    public async Task<(PersonaUsuarioDTO persona, string mensaje, string tipo)> ObtenerDtoIdAsync(int personaId)
    {
        try
        {
            var persona = await _personaRepository.GetDetalleByIdAsync(personaId);

            if (persona == null)
                return (null, "La persona no existe.", "danger");

            return (persona, "Persona obtenida correctamente.", "success");
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
            var persona = await _personaRepository.GetByIdAsync(id);
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
        var personaActual = await _personaRepository.GetByIdAsync(persona.PersonaId);

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

        //? Validacion de Roles
        bool esPropietario = persona.TipoPersona.Contains("propietario");
        bool esInquilino = persona.TipoPersona.Contains("inquilino");
        bool esEmpleado = persona.TipoPersona.Contains("empleado");

        var propietario = await _propietarioRepository.GetByIdAsync(personaActual.PersonaId);
        var inquilino = await _inquilinoRepository.GetByIdAsync(personaActual.PersonaId);
        var empleado = await _empleadoRepository.GetByIdAsync(personaActual.PersonaId);

        //- PROPIETARIO
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
            // No hagas nada si ya está habilitado y sigue seleccionado
        }
        else if (propietario != null && propietario.Estado)
        {
            await _propietarioRepository.UpdateAsync(propietario.PropietarioId, false);
            notificaciones.Add("Perfil propietario deshabilitado correctamente");
        }



        //- INQUILINO
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

        //- Empleado
        if (esEmpleado)
        {
            if (empleado == null)
            {
                await _empleadoRepository.AddAsync(personaActual.PersonaId);
                notificaciones.Add("Perfil empleado asignado correctamente");
            }
            else if (!empleado.Estado)
            {
                await _empleadoRepository.UpdateAsync(empleado.EmpleadoId, true);
                notificaciones.Add("Perfil empleado habilitado correctamente");
            }
        }
        else if (empleado != null && empleado.Estado)
        {
            await _empleadoRepository.UpdateAsync(empleado.EmpleadoId, false);
            notificaciones.Add("Perfil empleado deshabilitado correctamente");
        }


        if (notificaciones.Count == 0)
            return (true, "No hubo cambios.", "info");

        var tipo = notificaciones.Any(n => n.Contains("deshabilitado")) ? "danger" : "success";
        return (true, string.Join(". ", notificaciones), tipo);
    }

    public async Task<bool> EsEmpleado(int personaId)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(personaId);
        return empleado != null && empleado.Estado;
    }

    public async Task<(DatosPersonalesDTO datos, bool exito)> ObtenerDatosPersonalesByEmailAsync(string email)
    {
        try
        {
            var data = await _personaRepository.GetDatosPersonalesByEmailAsync(email);

            if (data == null)
                return (null, false);

            return (data, true);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los datos personales por email", ex);
        }
    }

    public async Task<(DatosPersonalesDTO datos, bool exito)> ActualizarDatosPersonalesAsync(DatosPersonalesDTO datos)
    {
        try
        {
            var data = await _personaRepository.UpdateDatosPersonalesAsync(datos);
            if (data == null)
                return (null, false);
                
            return (data, true);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar los datos personales", ex);
        }
    }

    public async Task<bool> CambiarPasswordAsync(CambiarPasswordDTO cambiarPasswordDTO, string email)
    {
        try
        {
            var passwordActual = await _personaRepository.GetPasswordByEmailAsync(email);
            var hashActual = PasswordHelper.HashPassword(cambiarPasswordDTO.PasswordActual, _configuration["Salt"]);

            // Validar contraseña enviada con la de la base de datos
            if (hashActual != passwordActual)
                return false;

            //Validar nueva contraseña y confirmación
            if (cambiarPasswordDTO.PasswordNueva != cambiarPasswordDTO.PasswordConfirmar)
                return false;

            // Hashear la nueva contraseña y actualizar
            var passwordNueva = PasswordHelper.HashPassword(cambiarPasswordDTO.PasswordNueva, _configuration["Salt"]);

            var data = await _usuarioRepository.UpdatePasswordAsync(passwordNueva, email);

            if (!data)
                return false;

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al cambiar la contraseña", ex);
        }
    }
}