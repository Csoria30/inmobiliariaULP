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
                var (exito, mensaje, tipo) = await _personaService.CrearAsync(persona);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

                if (exito)
                    return RedirectToAction(nameof(Index));
                else
                    return View("Create", persona); // Volver al formulario con datos
            }

            // Si ModelState no es válido, retornar a la vista con los datos ingresados
            return View("Create", persona);
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
                var (exito, mensaje, tipo) = await _personaService.EditarAsync(persona);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;
                return RedirectToAction(nameof(Index));
            }
            TempData["Notificacion"] = "Error al editar la persona.";
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