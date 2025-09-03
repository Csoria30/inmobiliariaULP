using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1.Crmf; //  ValidationException

namespace inmobiliariaULP.Controllers;

public class PersonaController : Controller
{
    private readonly ILogger<PersonaController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public PersonaController(ILogger<PersonaController> logger)
    {
        _logger = logger;
        _personaService = new PersonaServiceImpl();
        _inquilinoService = new InquilinoServiceImpl();
        _propietarioService = new PropietarioServiceImpl();
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var personas = await _personaService.ObtenerTodosAsync();
            return View(personas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View(new List<Persona>()); // Vista vacía con error
        }
    }

    //* GET: PersonasController/Create
    [HttpGet]
    public IActionResult Create()
    {
        try
        {


            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }

    //! POST: PersonasController/Create
    [HttpPost]
    [ValidateAntiForgeryToken] // Buena práctica para prevenir ataques CSRF
    public async Task<IActionResult> Create(Persona persona)
    {
        try
        {
            if (ModelState.IsValid)
            {

                // Crea la persona y obtiene el ID generado
                var personaId = await _personaService.NuevoAsync(persona);

                // Obntiene el tipo de persona desde el formulario
                if (persona.TipoPersona == null || persona.TipoPersona.Count == 0 || !persona.TipoPersona.Any())
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un perfil (Inquilino y/o Propietario).");
                    return View(persona);
                }

                foreach (var tipo in persona.TipoPersona)
                {
                    switch (tipo)
                    {
                        case "inquilino":
                            await _inquilinoService.NuevoAsync(personaId);
                            break;
                        case "propietario":
                            await _propietarioService.NuevoAsync(personaId);
                            break;
                        default:
                            throw new ValidationException("Tipo de persona inválido.");
                    }
                }


                TempData["Notificacion"] = "Persona creada correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                Console.WriteLine("ModelState NO es válido"); // <-- Esto indica que falla validación
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }
                return View(persona);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al Crear personas");
            TempData["Error"] = "Error al Crear la persona: " + ex.Message;
            return View("Error");
        }
    }


    //* GET: PersonasController/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var persona = await _personaService.ObtenerIdAsync(id);

            if (persona != null){
                if (persona.Estado == false)
                {
                    TempData["Notificacion"] = "No se puede editar una persona deshabilitada.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Inicializamos la lista de tipos vacía si es null
            if (persona.TipoPersona == null)
                persona.TipoPersona = new List<string>();

            return View("Create", persona);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }

    //! POST: PersonasController/Edit    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Persona persona)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var personaActual = await _personaService.ObtenerIdAsync(persona.PersonaId);

                if (personaActual != null)
                {
                    // Lista de propiedades
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

                    if (datosPersonalesCambiaron)
                    {
                        // Obtenemos los datos de las personas
                        personaActual.Nombre = persona.Nombre;
                        personaActual.Apellido = persona.Apellido;
                        personaActual.Dni = persona.Dni;
                        personaActual.Telefono = persona.Telefono;
                        personaActual.Email = persona.Email;

                        // Guardamos los cambios en la base de datos - Tabla personas
                        await _personaService.ActualizarAsync(personaActual);

                        TempData["Notificacion"] = "Datos personales actualizados correctamente";
                        TempData["NotificacionTipo"] = "success";
                    }

                    

                    

                    //- PROPIETARIO
                    var propietario = await _propietarioService.ObtenerIdAsync(personaActual.PersonaId);
                    bool esPropietario = persona.TipoPersona.Contains("propietario");

                    if (esPropietario)
                    {
                        if (propietario == null)
                        {
                            await _propietarioService.NuevoAsync(personaActual.PersonaId);
                            TempData["Notificacion"] = "Perfil propietario asignado correctamente";
                            TempData["NotificacionTipo"] = "success";
                        }
                        else if (!propietario.Estado)
                        {
                            // Habilitar solo si estaba deshabilitado
                            await _propietarioService.ActualizarAsync(propietario.PropietarioId, true);
                            TempData["Notificacion"] = "Perfil propietario habilitado correctamente";
                            TempData["NotificacionTipo"] = "success";
                        }
                        // Si ya está habilitado, no hagas nada
                    }
                    else if (propietario != null && propietario.Estado)
                    {
                        // Deshabilitar solo si estaba habilitado
                        await _propietarioService.ActualizarAsync(propietario.PropietarioId, false); 
                        TempData["Notificacion"] = "Perfil propietario deshabilitado correctamente";
                        TempData["NotificacionTipo"] = "danger";
                    }

                    //- INQUILINO
                    var inquilino = await _inquilinoService.ObtenerIdAsync(personaActual.PersonaId);
                    bool esInquilino = persona.TipoPersona.Contains("inquilino");

                    if (esInquilino)
                    {
                        if (inquilino == null)
                        {
                            await _inquilinoService.NuevoAsync(personaActual.PersonaId);
                            TempData["Notificacion"] = "Perfil inquilino asignado correctamente";
                            TempData["NotificacionTipo"] = "success";
                        }
                        else if (!inquilino.Estado)
                        {
                            // Habilitar solo si estaba deshabilitado
                            await _inquilinoService.ActualizarAsync(inquilino.InquilinoId, true);
                            TempData["Notificacion"] = "Perfil inquilino habilitado correctamente";
                            TempData["NotificacionTipo"] = "success";
                        }
                        // Si ya está habilitado, no hagas nada
                    }
                    else if (inquilino != null && inquilino.Estado)
                    {
                        // Deshabilitar solo si estaba habilitado
                        await _inquilinoService.ActualizarAsync(inquilino.InquilinoId, false);
                        TempData["Notificacion"] = "Perfil inquilino deshabilitado correctamente";
                        TempData["NotificacionTipo"] = "danger";
                    }

                    return RedirectToAction(nameof(Index));
                }//. Persona null
            }
            
            // Si ModelState no es válido, redirige igual o muestra error
            TempData["Notificacion"] = "Error al eliminar la persona.";
            TempData["NotificacionTipo"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar persona {PersonaId}", persona.PersonaId);
            TempData["Error"] = "Error al guardar los cambios.";
            return View("Create", persona);
        }

    }


    //* GET: PersonasController/Details
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var (persona, mensaje, tipo) = await _personaService.ObtenerDetalleAsync(id);

            if (persona == null)
            {
                TempData["Error"] = "La persona no existe.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SoloLectura = true; // Flag para la vista
            return View("Create", persona); // Reutilizamos la vista Create

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }

    //! POST: PersonasController/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (exito, mensaje, tipo) = await _personaService.CambiarEstadoAsync(id);
            TempData["Notificacion"] = mensaje;
            TempData["NotificacionTipo"] = tipo;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar persona {PersonaId}", id);
            TempData["Notificacion"] = "Error al eliminar la persona.";
            TempData["NotificacionTipo"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }
    
}