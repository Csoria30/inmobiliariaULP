using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using inmobiliariaULP.Helpers; // Para ModelStateHelper
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]

namespace inmobiliariaULP.Controllers;

[Authorize]
public class InmuebleController : Controller
{
    private readonly ILogger<InmuebleController> _logger;
    private readonly IPersonaService _personaService;
    private readonly ITipoService _tipoService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;
    private readonly IInmuebleService _inmuebleService;

    public InmuebleController(
        ILogger<InmuebleController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        ITipoService tipoService,
        IInmuebleService inmuebleService)
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _tipoService = tipoService;
        _inmuebleService = inmuebleService;
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
            return View(new Inmueble());
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
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

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

                estado = inmueble.Estado.HasValue && inmueble.Estado.Value
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

                    <a 
                        href='#' 
                        class='btn btn-sm {(inmueble.Estado == true ? "btn-outline-danger" : "btn-outline-success")}'
                        data-bs-toggle='modal'
                        data-bs-target='#modalEliminarInmueble'
                        data-inmuebledid='{inmueble.InmuebleId}'
                        data-inmueblednombre='{inmueble.Direccion}'
                        data-inmuebleestado='{inmueble.Estado}'
                        data-bs-placement='top'
                        title='{(inmueble.Estado == true ? "Deshabilitar" : "Habilitar")}'>
                        <i class='bi {(inmueble.Estado == true ? "bi-trash3" : "bi-arrow-repeat")}'></i>
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
    //! POST: InmuebleController/Edit
    [HttpPost]
    public async Task<IActionResult> Edit(Inmueble inmueble)
    {
        try
        {

            if (ModelState.IsValid)
            {
                var (exito, mensaje, tipo) = await _inmuebleService.EditarAsync(inmueble);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

                if (exito)
                    return RedirectToAction(nameof(Index));
            }

            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                }
            }

            //Si el modelo no es valido, retornar los tipos y la vista
            var tipos = await _tipoService.ObtenerTodosAsync();
            ViewBag.Tipos = tipos;


            return View("Create", inmueble);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el inmueble");
            TempData["Error"] = "Error al actualizar el inmueble: " + ex.Message;
            return View("Error");
        }
    }

    //! POST: InmuebleController/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (exito, mensaje, tipo) = await _inmuebleService.CambiarEstadoAsync(id);
            TempData["Notificacion"] = mensaje;
            TempData["NotificacionTipo"] = tipo;

            ViewBag.eliminacion = true;

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el inmueble");
            TempData["Error"] = "Error al eliminar el inmueble: " + ex.Message + $" (ID: {id})";
            return View("Error");
        }
    }


    //* GET: InmuebleController/BuscarDisponibles
    [HttpGet]
    public async Task<IActionResult> BuscarDisponibles()
    {
        try
        {
            var viewModel = new InmuebleDisponibilidadDTO
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddYears(1),

                // Inicializar lista vacía para los resultados
                Resultados = new List<InmuebleDisponibilidadDTO>()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de búsqueda de inmuebles");
            return View("Error");
        }
    }

    //! POST: InmuebleController/BuscarDisponibles
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuscarDisponibles(InmuebleDisponibilidadDTO modelo)
    {
        try
        {
            //? Validacion modelo - para debbuging
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Errores de validación en el formulario");
                modelo.Resultados = new List<InmuebleDisponibilidadDTO>();
                return View(modelo);
            }
            
            
            var inmueblesDisponibles = await _inmuebleService.BuscarDisponiblesAsync(
                modelo.FechaInicio!.Value, 
                modelo.FechaFin!.Value, 
                modelo.Uso, 
                modelo.Ambientes?.ToString(), 
                modelo.PrecioMin?.ToString(), 
                modelo.PrecioMax?.ToString()
            );

            // ASIGNAR resultados al modelo
            modelo.Resultados = inmueblesDisponibles.ToList();
            
            // MENSAJE de resultado
            if (modelo.Resultados.Count > 0)
            {
                TempData["Success"] = $"Se encontraron {modelo.Resultados.Count} inmuebles disponibles para las fechas seleccionadas.";
            }
            else
            {
                TempData["Info"] = "No se encontraron inmuebles disponibles para los criterios seleccionados.";
            }

            return View(modelo);
        }
        catch (ArgumentException ex)
        {          
            // Agregar el error específico al ModelState para mostrarlo en la vista
            if (ex.Message.Contains("fecha"))
            {
                ModelState.AddModelError("FechaFin", ex.Message);
            }
            else if (ex.Message.Contains("precio"))
            {
                ModelState.AddModelError("PrecioMax", ex.Message);
            }
            else
            {
                ModelState.AddModelError("", ex.Message);
            }
            
            modelo.Resultados = new List<InmuebleDisponibilidadDTO>();
            return View(modelo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda de inmuebles");
            TempData["Error"] = "Error al buscar inmuebles: " + ex.Message;
            
            modelo.Resultados = new List<InmuebleDisponibilidadDTO>();
            return View(modelo);
        }
    }

}