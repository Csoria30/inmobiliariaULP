using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations; //  ValidationException

namespace inmobiliariaULP.Controllers;

public class PersonaController : Controller
{
    private readonly ILogger<PersonaController> _logger;
    private readonly IPersonaService _personaService;

    public PersonaController(ILogger<PersonaController> logger)
    {
        _logger = logger;
        _personaService = new PersonaServiceImpl();
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
            return View(new List<Persona>()); // Vista vacía con error
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
                var personas = await _personaService.NuevoAsync(persona);
                return RedirectToAction("Index"); 
            }
            else
            {
                return View();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View(new List<Persona>()); // Vista vacía con error
        }
    }

}
