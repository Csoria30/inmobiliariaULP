using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public PagoController(ILogger<PagoController> logger, IPersonaService personaService, IInquilinoService inquilinoService, IPropietarioService propietarioService)
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    // Agrega aquí tus acciones adicionales...
}