using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaULP.Models;
using inmobiliariaULP.Services.Interfaces;
using inmobiliariaULP.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]

namespace inmobiliariaULP.Controllers;

[Authorize]
public class PropietarioController : Controller
{
    private readonly ILogger<PersonaController> _logger;
    private readonly IPersonaService _personaService;
    private readonly IInquilinoService _inquilinoService;
    private readonly IPropietarioService _propietarioService;

    public PropietarioController(
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
            _logger.LogError(ex, "Error al obtener propietarios");
            TempData["Error"] = "Error al cargar las propietarios: " + ex.Message;
            return View(new List<Propietario>()); // Vista vacía con error
        }
    }

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
            var (personas, total) = await _propietarioService.ObtenerTodosAsync(page, pageSize, searchValue);

            // URL de retorno para los botones de acción
            var returnUrl = Url.Action("Index", "Propietario");

            // Arma los datos para DataTables
            var data = personas.Select(persona => new
            {
                dni = persona.Dni,
                apellido = persona.Apellido,
                nombre = persona.Nombre,
                telefono = persona.Telefono,

                estado = persona.Estado
                    ? "<span class='badge bg-success'>Habilitado</span>"
                    : "<span class='badge bg-danger'>Deshabilitado</span>",

                acciones = $@"
                <div class='btn-group' role='group'>
                    
                    <a 
                        href='/Persona/Details/{persona.PersonaId}?returnUrl={returnUrl}' 
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
                

                </div>"


            });

            return Json(new
            {
                draw = draw,
                recordsTotal = total,
                recordsFiltered = total, // Si implementas búsqueda, cambia este valor
                data = data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener datos para DataTable");
            return StatusCode(500, "Error al procesar la solicitud.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(string term)
    {
        var propietarios = await _propietarioService.ListarActivosAsync(term);

        // _logger.LogInformation("Propietarios encontrados: {@Propietarios}", propietarios);

        var resultado = propietarios.Select(p => new
        {
            id = p.PropietarioId,
            text = $"{p.Apellido}, {p.Nombre}"
        });

        return Json(resultado);
    }

}
