using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

[Authorize]
public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;
    private readonly IUsuarioService _usuarioService;
    private readonly IEmpleadoService _empleadoService;

    public ContratoController(
        ILogger<ContratoController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        IUsuarioService usuarioService,
        IEmpleadoService empleadoService
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _usuarioService = usuarioService;
        _empleadoService = empleadoService;
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    // Agrega aquí tus acciones adicionales...
}