using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;

namespace inmobiliariaULP.Controllers;

public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private readonly IPersonaService _personaService;
    private readonly ITipoService _tipoService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;
    private readonly IInmuebleService _inmuebleService;

    public InmuebleController(ILogger<InmuebleController> logger)
    {
        _logger = logger;
        _personaService = new PersonaServiceImpl();
        _inquilinoService = new InquilinoServiceImpl();
        _propietarioService = new PropietarioServiceImpl();
        _tipoService = new TipoServiceImpl();
        _inmuebleService = new InmuebleServiceImpl();
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    //* GET: PersonasController/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        try
        {
            var tipos = await _tipoService.ObtenerTodosAsync();
            ViewBag.Tipos = tipos;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }

    }

    //! POST: InmuebleController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inmueble inmueble)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var (exito, mensaje, tipo) = await _inmuebleService.CrearAsync(inmueble);
                TempData["Mensaje"] = mensaje;
                TempData["Tipo"] = tipo;

                if (exito)
                    return RedirectToAction(nameof(Index));

            }

            //Si el modelo no es valido, retornar los tipos y la vista
            var tipos = await _tipoService.ObtenerTodosAsync();
            ViewBag.Tipos = tipos;
            return View("Create", inmueble);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el inmueble");
            TempData["Error"] = "Error al crear el inmueble: " + ex.Message;
            return View("Error");
        }
    }
}