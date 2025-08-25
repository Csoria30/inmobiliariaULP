using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;

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

}
