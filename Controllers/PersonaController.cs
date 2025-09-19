using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1.Crmf; //  ValidationException

namespace inmobiliariaULP.Controllers;

public class PersonaController : Controller
{
    private readonly ILogger<PersonaController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public PersonaController(
        ILogger<PersonaController> logger,
        IPersonaService personaService,
        IInquilinoService inquilinoService,
        IPropietarioService propietarioService
    )
    {
        _logger = logger;
        _personaService = personaService;
        _inquilinoService = inquilinoService;
        _propietarioService = propietarioService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View(new List<Persona>()); // Vista vacía con error
        }
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


    //! POST: PersonasController/Create
    [HttpPost]
    [ValidateAntiForgeryToken] // Buena práctica para prevenir ataques CSRF
    public async Task<IActionResult> Create(Persona persona)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var (exito, mensaje, tipo) = await _personaService.CrearAsync(persona);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;

                if (exito)
                    return RedirectToAction(nameof(Index));
                else
                    return View("Create", persona); // Volver al formulario con datos
            }

            // Si ModelState no es válido, retornar a la vista con los datos ingresados
            return View("Create", persona);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al Crear personas");
            TempData["Error"] = "Error al Crear la persona: " + ex.Message;
            return View("Error");
        }
    }


    //* GET: PersonasController/Edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var persona = await _personaService.ObtenerIdAsync(id);

            if (persona != null)
            {
                if (persona.Estado == false)
                {
                    TempData["Notificacion"] = "No se puede editar una persona deshabilitada.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Inicializamos la lista de tipos vacía si es null
            if (persona.TipoPersona == null)
                persona.TipoPersona = new List<string>();

            return View("Create", persona);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }
 

    //! POST: PersonasController/Edit    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Persona persona)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var (exito, mensaje, tipo) = await _personaService.EditarAsync(persona);
                TempData["Notificacion"] = mensaje;
                TempData["NotificacionTipo"] = tipo;
                return RedirectToAction(nameof(Index));
            }
            TempData["Notificacion"] = "Error al editar la persona.";
            TempData["NotificacionTipo"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar persona {PersonaId}", persona.PersonaId);
            TempData["Error"] = "Error al guardar los cambios.";
            return View("Create", persona);
        }

    }


    //* GET: PersonasController/Details
    public async Task<IActionResult> Details(int id, string? returnUrl = null)
    {
        try
        {
            var (persona, mensaje, tipo) = await _personaService.ObtenerDetalleAsync(id);

            if (persona == null)
            {
                TempData["Error"] = "La persona no existe.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SoloLectura = true; // Flag para la vista
            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Index", "Persona"); //Identificar de donde se llamo

            return View("Create", persona);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener personas");
            TempData["Error"] = "Error al cargar las personas: " + ex.Message;
            return View("Error");
        }
    }

    //! POST: PersonasController/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (exito, mensaje, tipo) = await _personaService.CambiarEstadoAsync(id);
            TempData["Notificacion"] = mensaje;
            TempData["NotificacionTipo"] = tipo;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar persona {PersonaId}", id);
            TempData["Notificacion"] = "Error al eliminar la persona.";
            TempData["NotificacionTipo"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }


    [HttpPost]
    public async Task<IActionResult> ObtenerDataTable()
    {
        try
        {
            // Recibe parámetros de DataTables
            var draw = Request.Form["draw"].FirstOrDefault();
            // Es un número que envía DataTables en cada petición AJAX para identificar la respuesta y sincronizar el frontend.

            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            //  Es el índice del primer registro que DataTables quiere mostrar (para paginación).

            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            //  Es la cantidad de registros que DataTables quiere mostrar por página (tamaño de página).

            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            //  Es el valor de búsqueda que el usuario ha ingresado en el cuadro de búsqueda de DataTables.

            int page = (start / length) + 1;
            //  Es el número de página actual que DataTables está solicitando.

            int pageSize = length;
            //  Es la cantidad de registros por página que DataTables está solicitando.

            // Llama a tu servicio para obtener datos paginados y filtrados
            var (personas, total) = await _personaService.ObtenerTodosAsync(page, pageSize, searchValue);

            // Arma los datos para DataTables
            var data = personas.Select(persona => new {
                dni = persona.Dni,
                apellido = persona.Apellido,
                nombre = persona.Nombre,
                telefono = persona.Telefono,
                perfil = string.Join(" ", persona.TipoPersona.Select(tipo =>
                    tipo == "inquilino"
                        ? "<span class='badge bg-primary me-1'>Inquilino</span>"
                    
                    : tipo == "propietario"
                            ? "<span class='badge bg-warning text-dark me-1'>Propietario</span>"

                    : $"<span class='badge bg-secondary me-1'>{tipo}</span>"
                )),

                estado = persona.Estado
                    ? "<span class='badge bg-success'>Habilitado</span>"
                    : "<span class='badge bg-danger'>Deshabilitado</span>",

                acciones = $@"
                <div class='btn-group' role='group'>
                    
                    <a 
                        href='/Persona/Details/{persona.PersonaId}' 
                        class='btn btn-sm btn-outline-info' 
                        data-bs-toggle='tooltip' 
                        data-bs-placement='top' 
                        title='Más información'>
                        <i class='bi bi-eye'></i>
                    </a>
                
                    <a 
                        href='/Persona/Edit/{persona.PersonaId}' 
                        class='btn btn-sm btn-outline-warning' 
                        data-bs-toggle='tooltip' 
                        data-bs-placement='top' 
                        title='Editar Persona'>
                        <i class='bi bi-pencil'></i>
                    </a>
                
                    <a 
                        href='/Persona/Delete/{persona.PersonaId}' 
                        class='btn btn-sm {(persona.Estado ? "btn-outline-danger" :   "btn-outline-success")}'
                        data-bs-toggle='modal'
                        data-bs-target='#modalEliminarPersona'
                        data-personaid='{persona.PersonaId}'
                        data-personanombre='{persona.Apellido}, {persona.Nombre}'
                        data-bs-placement='top'
                        title='{(persona.Estado ? "Deshabilitar" : "Habilitar")}'>
                        <i class='bi {(persona.Estado ? "bi-trash3" : "bi-arrow-repeat")}'></i>
                    </a>

                </div>"
            });

            return Json(new
            {
                draw = draw,
                recordsTotal = total,
                recordsFiltered = total, // Si implementas búsqueda, cambia este valor
                data = data
            });
        }catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos para DataTable");
            return StatusCode(500, "Error al procesar la solicitud.");
        }
    }
}