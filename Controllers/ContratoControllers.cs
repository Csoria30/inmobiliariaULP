using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using inmobiliariaULP.Models.ViewModels;

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
    private readonly IContratoService _contratoService;

    public ContratoController(
        ILogger<ContratoController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        IUsuarioService usuarioService,
        IEmpleadoService empleadoService,
        IContratoService contratoService
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _usuarioService = usuarioService;
        _empleadoService = empleadoService;
        _contratoService = contratoService;
    }

    // Ejemplo de acción Index
    public IActionResult Index()
    {
        return View();
    }

    //! POST: ContratoController/Listar
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
            var (contratos, total) = await _contratoService.ObtenerTodosAsync(page, pageSize, searchValue);

            var data = contratos.Select(contrato => new
            {
                contratoId = contrato.ContratoId,
                direccion = contrato.Direccion,
                tipoInmueble = contrato.TipoInmueble,
                nombrePropietario = contrato.NombrePropietario,
                nombreInquilino = contrato.NombreInquilino,
                fechaInicio = contrato.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = contrato.FechaFin.ToString("dd/MM/yyyy"),
                montoMensual = contrato.MontoMensual.ToString("C"),
                estadoContrato = contrato.EstadoContrato == "vigente"
                    ? "<span class='badge bg-success'>Vigente</span>"
                    : contrato.EstadoContrato == "rescindido"
                        ? "<span class='badge bg-danger'>Rescindido</span>"
                        : "<span class='badge bg-secondary'>" + contrato.EstadoContrato + "</span>",
                pagosRealizados = contrato.PagosRealizados,

                acciones = $@"
                    <div class='btn-group' role='group'>
                        <a 
                            href='/Contrato/Details/{contrato.ContratoId}' 
                            class='btn btn-sm btn-outline-info' 
                            data-bs-toggle='tooltip' 
                            data-bs-placement='top' 
                            title='Más información'>
                            <i class='bi bi-eye'></i>
                        </a>
                        <a 
                            href='/Contrato/Edit/{contrato.ContratoId}' 
                            class='btn btn-sm btn-outline-warning' 
                            data-bs-toggle='tooltip' 
                            data-bs-placement='top' 
                            title='Editar Contrato'>
                            <i class='bi bi-pencil'></i>
                        </a>
                        <a 
                            href='#' 
                            class='btn btn-sm btn-outline-danger'
                            data-bs-toggle='modal'
                            data-bs-target='#modalEliminarContrato'
                            data-contratoid='{contrato.ContratoId}'
                            data-contratonombre='{contrato.Direccion}'
                            data-bs-placement='top'
                            title='Eliminar'>
                            <i class='bi bi-trash3'></i>
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
            _logger.LogError(ex, "Error al obtener contratos");
            TempData["Error"] = "Error al cargar los contratos: " + ex.Message;
            return View(new List<Contrato>()); // Vista vacía con error
        }
    }

    //* GET: ContratoController/Details
    public async Task<IActionResult> Details(int id)
    {
        var contrato = await _contratoService.GetByIdAsync(id);
        if (contrato == null)
        {
            TempData["Error"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.SoloLectura = true; // Flag para la vista

        return View("Create", contrato);
    }

}