using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]

namespace inmobiliariaULP.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly ILogger<PerfilController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public PerfilController(
        ILogger<PerfilController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService    ;
        _propietarioService = propietarioService;
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    // Agrega aquí tus acciones adicionales...
}