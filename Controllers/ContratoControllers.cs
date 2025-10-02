using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using inmobiliariaULP.Models.ViewModels;
using inmobiliariaULP.Helpers;

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
    private readonly IInmuebleService _inmuebleService;

    public ContratoController(
        ILogger<ContratoController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService,
        IUsuarioService usuarioService,
        IEmpleadoService empleadoService,
        IContratoService contratoService,
        IInmuebleService inmuebleService
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
        _usuarioService = usuarioService;
        _empleadoService = empleadoService;
        _contratoService = contratoService;
        _inmuebleService = inmuebleService;
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
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var contrato = await _contratoService.ObtenerPorIdAsync(id);
        if (contrato == null)
        {
            TempData["Error"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.SoloLectura = true; // Flag para la vista

        return View("Create", contrato);
    }

    //! POST: ContratoController/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContratoDetalleDTO contrato)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelStateHelper.GetErrors(ModelState);
            }

            if (ModelState.IsValid)
            {
                var (exito, mensaje, tipo) = await _contratoService.EditarAsync(contrato);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

                if (exito)
                    return RedirectToAction(nameof(Index));
                
                return View("Create", contrato);
            }


            return View("Create", contrato);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar el contrato");
            TempData["Error"] = "Error al editar el contrato: " + ex.Message;
            return View("Error");
        }
    }

    //* GET: ContratoController/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var contratoActual = await _contratoService.ObtenerPorIdAsync(id);

        if (contratoActual == null)
        {
            TempData["Error"] = "El contrato no existe.";
            return RedirectToAction(nameof(Index));
        }

        var emailUsuario = User.Identity.Name;
        var (exito, mensaje, usuario) = await _usuarioService.ObtenerPerfilAsync(emailUsuario);

        if (exito && usuario != null)
        {
            contratoActual.UsuarioId = usuario.UsuarioId;
            contratoActual.NombreEmpleado = $"{usuario.Apellido} {usuario.Nombre}";
            contratoActual.EmailUsuario = usuario.Email;
            contratoActual.RolUsuario = usuario.Rol;
            contratoActual.EmailUsuario = usuario.Email;
        }

        ViewBag.SoloLectura = false; // Flag para la vista

        return View("Create", contratoActual);
    }


    //! POST: ContratoController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContratoDetalleDTO contrato)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View("Create", contrato);
            }

            if (ModelState.IsValid)
            {

                var (exito, mensaje, tipo) = await _contratoService.CrearAsync(contrato);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

                if (exito)
                    return RedirectToAction(nameof(Index));
            }


            return View("Create", contrato);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el inmueble");
            TempData["Error"] = "Error al crear el inmueble: " + ex.Message;
            return View("Error");
        }
    }

    //* GET: ContratoController/Create
    [HttpGet]
    public async Task<IActionResult> Create(int? inmuebleId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        try
        {
            // Crear una sola instancia del modelo
            var model = new ContratoDetalleDTO();

            //- INMUEBLE
            //Obtener datos del inmueble que viene por parámetro
            if (inmuebleId.HasValue)
            {
                //Datos inmueble
                var (inmueble, _, _) = await _inmuebleService.ObtenerIdAsync(inmuebleId.Value);

                model.InmuebleId = inmueble.InmuebleId;
                model.PropietarioId = inmueble.PropietarioId ?? 0;
                model.Coordenadas = inmueble.Coordenadas;
                model.Direccion = inmueble.Direccion;
                model.Ambientes = inmueble.Ambientes ?? 0;
                model.TipoInmueble = inmueble.TipoDescripcion;
                model.UsoInmueble = inmueble.Uso;
                model.NombrePropietario = inmueble.PropietarioNombre;


                //- PROPIETARIO
                if (inmueble.PropietarioId.HasValue && inmueble.PropietarioId.Value > 0)
                {
                    var propietario = await _propietarioService.ObtenerIdAsync(inmueble.PropietarioId.Value);
                    if (propietario != null)
                    {
                        //-Persona
                        var personaPropietario = await _personaService.ObtenerIdAsync(propietario.PersonaId);
                        model.EmailPropietario = personaPropietario.Email;
                        model.TelefonoPropietario = personaPropietario.Telefono;
                    }   
                }
                else
                {
                    model.EmailPropietario = string.Empty;
                    model.TelefonoPropietario = string.Empty;
                }
            }

            //Fechas - Si vienen por parametros
            if (fechaInicio.HasValue)
            {
                model.FechaInicio = fechaInicio.Value;
            }
            else
            {
                model.FechaInicio = DateTime.Today;
            }

            if (fechaFin.HasValue)
            {
                model.FechaFin = fechaFin.Value;
            }
            else
            {
                model.FechaFin = DateTime.Today.AddDays(30);
            }

            
            
            //-USUARIO
            // Obtener información del usuario logueado
            var emailUsuario = User.Identity.Name;
            var (exito, mensaje, usuario) = await _usuarioService.ObtenerPerfilAsync(emailUsuario);

            // Configurar información del usuario si se obtuvo correctamente
            if (exito && usuario != null)
            {
                model.UsuarioId = usuario.UsuarioId;
                model.NombreEmpleado = $"{usuario.Apellido} {usuario.Nombre}";
                model.EmailUsuario = usuario.Email;
                model.RolUsuario = usuario.Rol;
            }

            // Configurar valores por defecto
            model.EstadoContrato = "vigente";

            return View("Create", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la vista de crear contrato");
            TempData["Error"] = "Error al cargar el formulario: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    //!POST: ContratoController/BuscarHabilitados
    [HttpGet]
    public async Task<IActionResult> BuscarHabilitados(string term)
    {
        try
        {
            var inmuebles = await _inmuebleService.ListarActivosAsync(term);

            var resultado = inmuebles.Select(i => new
            {
                InmuebleId = i.InmuebleId,
                text = i.Direccion,
                TipoInmueble = i.TipoInmueble,
                UsoInmueble = i.UsoInmueble,
                Ambientes = i.Ambientes,
                Coordenadas = i.Coordenadas,
                PrecioBase = i.PrecioBase,
                EstadoInmueble = i.EstadoInmueble,
                PropietarioId = i.PropietarioId,
                NombrePropietario = i.NombrePropietario,
                EmailPropietario = i.EmailPropietario,
                TelefonoPropietario = i.TelefonoPropietario
            });

            return Json(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar inmuebles habilitados");
            return Json(new { error = "Error al buscar inmuebles habilitados: " + ex.Message });
        }
    }

    //!POST: ContratoController/BuscarHabilitadosInquilinos
    [HttpGet]
    public async Task<IActionResult> BuscarHabilitadosInquilinos(string term)
    {
        try
        {
            var inquilinos = await _inquilinoService.ListarActivosAsync(term);

            var resultado = inquilinos.Select(i => new
            {
                inquilinoId = i.InquilinoId,
                text = $"{i.NombreInquilino} - {i.Dni}", // Para mostrar nombre y DNI en el select
                dni = i.Dni,
                nombreInquilino = i.NombreInquilino,
                email = i.Email,
                telefono = i.Telefono
            });

            return Json(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar inmuebles habilitados");
            return Json(new { error = "Error al buscar inmuebles habilitados: " + ex.Message });
        }
    }

}