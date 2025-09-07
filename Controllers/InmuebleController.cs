using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
        _personaService = new PersonaServiceImpl();
        _inquilinoService = new InquilinoServiceImpl();
        _propietarioService = new PropietarioServiceImpl();
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
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
}