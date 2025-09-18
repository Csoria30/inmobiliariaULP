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

    //! POST: InmuebleController/Listar
    [HttpPost]
    public async Task<IActionResult> ObtenerDataTable()
    {
        try
        {
            // Leer parámetros de DataTables
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = Convert.ToInt32(HttpContext.Request.Form["start"].FirstOrDefault());
            var length = Convert.ToInt32(HttpContext.Request.Form["length"].FirstOrDefault());
            var searchValue = HttpContext.Request.Form["search[value]"].FirstOrDefault();

            // Calcular página actual
            var pageSize = length > 0 ? length : 10;
            var page = (start / pageSize) + 1;

            // Obtener datos paginados y filtrados
            var (inmuebles, total) = await _inmuebleService.ObtenerTodosAsync(page, pageSize, searchValue);

            var data = inmuebles.Select(inmueble => new
            {
                direccion = inmueble.Direccion,
                uso = inmueble.Uso,
                ambientes = inmueble.Ambientes,
                coordenadas = inmueble.Coordenadas,
                precioBase = inmueble.PrecioBase,
                descripcion = inmueble.TipoDescripcion,

                estado = inmueble.Estado == 1
                    ? "<span class='badge bg-success'>Habilitado</span>"
                    : "<span class='badge bg-danger'>Deshabilitado</span>",

                propietarioId = inmueble.PropietarioId,

                acciones = $@"
                <div class='btn-group' role='group'>
                    
                    <a 
                        href='/Inmueble/Details/{inmueble.InmuebleId}' 
                        class='btn btn-sm btn-outline-info' 
                        data-bs-toggle='tooltip' 
                        data-bs-placement='top' 
                        title='Más información'>
                        <i class='bi bi-eye'></i>
                    </a>
                
                    <a 
                        href='/Inmueble/Edit/{inmueble.InmuebleId}' 
                        class='btn btn-sm btn-outline-warning' 
                        data-bs-toggle='tooltip' 
                        data-bs-placement='top' 
                        title='Editar Inmueble'>
                        <i class='bi bi-pencil'></i>
                    </a>
                


                </div>"


            });

            var response = new
            {
                draw = draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = data
            };

            return Json(response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los inmuebles para DataTable");
            return StatusCode(500, "Error al procesar la solicitud");
        }
    }

    //* GET: InmuebleController/Details
    public async Task<IActionResult> Details(int id, string? returnUrl = null)
    {
        try
        {
            var (inmueble, mensaje, tipo) = await _inmuebleService.ObtenerIdAsync(id);

            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Inmueble");
            ViewBag.SoloLectura = true; // Flag para la vista

            return View("Create", inmueble);


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los detalles del inmueble");
            TempData["Error"] = "Error al obtener los detalles del inmueble: " + ex.Message;
            return View("Error");
        }
    }

    //* GET: InmuebleController/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var (inmueble, mensaje, tipo) = await _inmuebleService.ObtenerIdAsync(id);

            if (inmueble == null)
            {
                TempData["Error"] = "El inmueble no existe.";
                return RedirectToAction(nameof(Index));
            }
            
            //Si el modelo no es valido, retornar los tipos y la vista
            var tipos = await _tipoService.ObtenerTodosAsync();
            ViewBag.Tipos = tipos;
            
            return View("Create", inmueble);

        }
        catch
        {
            TempData["Error"] = "Error al obtener los detalles del inmueble.";
            return RedirectToAction(nameof(Index));
        }
    }
}