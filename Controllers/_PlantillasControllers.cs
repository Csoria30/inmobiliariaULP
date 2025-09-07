using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

public class NuevoController : Controller
{
    private readonly ILogger<NuevoController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public NuevoController(ILogger<NuevoController> logger)
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

    // Agrega aquí tus acciones adicionales...
}